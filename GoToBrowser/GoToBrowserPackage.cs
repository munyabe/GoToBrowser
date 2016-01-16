﻿using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using GoToBrowser.Configs;
using GoToBrowser.Properties;
using GoToBrowser.Utils;
using GoToBrowser.Views;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GoToBrowser
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "2.1", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidGoToBrowserPkgString)]
    public sealed partial class GoToBrowserPackage : Package
    {
        /// <summary>
        /// <c>Go to Brouser</c>の設定です。
        /// </summary>
        private ConfigContents _config = new ConfigContents();

        /// <summary>
        /// パッケージを初期化します。
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var commandService = this.GetService<IMenuCommandService, OleMenuCommandService>();
            var commandID = new CommandID(GuidList.guidGoToBrowserCmdSet, (int)PkgCmdIDList.configureCommand);
            commandService.AddCommand(new OleMenuCommand(ConfigureCallback, commandID));

            var solution = this.GetService<SVsSolution, IVsSolution>();
            uint solutionEventCoockie;
            solution.AdviseSolutionEvents(this, out solutionEventCoockie);
        }

        /// <summary>
        /// .suo ファイルから指定されたキーのデータを読み込みます。
        /// </summary>
        /// <remarks>
        /// .suo ファイルにキーが存在しない場合は呼び出されません。
        /// </remarks>
        protected override void OnLoadOptions(string key, Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                if (reader.PeekChar() < 0)
                {
                    return;
                }

                var first = reader.ReadString();
                if (0 <= reader.PeekChar())
                {
                    _config.MenuItems.Add(new CommandMenuItem(first, reader.ReadString(), (ExecuteMode)reader.ReadByte()));
                    while (0 <= reader.PeekChar())
                    {
                        _config.MenuItems.Add(new CommandMenuItem(reader.ReadString(), reader.ReadString(), (ExecuteMode)reader.ReadByte()));
                    }
                }
                else
                {
                    // MEMO : ver 1.2 までのデータフォーマットと互換性を維持するための対応
                    _config.MenuItems.Add(new CommandMenuItem("Go to Browser", first, ExecuteMode.ShowBrowser));
                }
            }
        }

        /// <summary>
        /// 指定されたキーでデータを .suo ファイルに保存します。
        /// </summary>
        protected override void OnSaveOptions(string key, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                _config.MenuItems.ForEach(menu =>
                {
                    writer.Write(menu.Name);
                    writer.Write(menu.UrlFormat);
                    writer.Write((byte)menu.Mode);
                });
            }

            base.OnSaveOptions(key, stream);
        }

        /// <summary>
        /// ドキュメントのカーソル位置の行数を取得します。
        /// </summary>
        private static int GetCurrentLineNumber(Document document)
        {
            var textDocument = document.Object("TextDocument") as TextDocument;
            return textDocument != null ? textDocument.Selection.ActivePoint.Line : 0;
        }

        /// <summary>
        /// 現在開いているソリューションのフルパスを取得します。
        /// プロジェクトファイルを直接開いている場合は、プロジェクトファイルのパスを取得します。
        /// </summary>
        private static string GetSolutionFullName(Solution solution)
        {
            var result = solution.FullName;
            if (string.IsNullOrEmpty(result))
            {
                foreach (var project in solution.Projects)
                {
                    dynamic comProject = project;
                    result = comProject.FullName;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// <c>Go to Browser</c>コマンドを実行したときの処理です。
        /// </summary>
        private void GoToBrowserCallback(object sender, EventArgs e)
        {
            try
            {
                var command = (MenuCommand)sender;
                ExecuteGoToBrowser(_config.MenuItems[PkgCmdIDList.GetCommandIndex(command.CommandID)]);
            }
            catch (Exception ex)
            {
                ShowMessageBox(string.Format(CultureInfo.CurrentCulture, "{0}", ex.Message), OLEMSGICON.OLEMSGICON_WARNING);
            }
        }

        /// <summary>
        /// <c>Go to Browser</c>の設定コマンドを実行したときの処理です。
        /// </summary>
        private void ConfigureCallback(object sender, EventArgs e)
        {
            var window = new MenuListView(_config.MenuItems);
            window.Applied += (applySender, applyArgs) =>
            {
                _config.MenuItems = applyArgs;

                var persistence = this.GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
                persistence.SavePackageUserOpts(this, ConfigContents.CONFIG_SUO_KEY);

                SetCommandVisible();
            };

            window.ShowDialog();
        }

        /// <summary>
        /// <c>Go to Browser</c>コマンドを実行します。
        /// </summary>
        private void ExecuteGoToBrowser(CommandMenuItem item)
        {
            var dte = this.GetService<DTE>();
            var document = dte.ActiveDocument;

            var macros = ConfigContents.CreateMacros(GetSolutionFullName(dte.Solution), document.FullName, GetCurrentLineNumber(document));
            var targetPath = StringUtil.Format(item.UrlFormat, macros);

            if (item.Mode == ExecuteMode.ShowBrowser)
            {
                dte.ExecuteCommand("navigate", string.Format("{0} /ext", Uri.EscapeUriString(targetPath)));
            }
            else if (item.Mode == ExecuteMode.Copy)
            {
                SetClipboard(Uri.EscapeUriString(targetPath));
            }
            else if (item.Mode == ExecuteMode.UnescapedCopy)
            {
                SetClipboard(targetPath);
            }
        }

        /// <summary>
        /// 指定の文字列をクリップボードにコピーします。
        /// </summary>
        private static void SetClipboard(string source)
        {
            try
            {
                Clipboard.SetText(source, TextDataFormat.Text);
            }
            catch (COMException)
            {
                // MEMO : 他のアプリケーションがクリップボードを監視している場合に発生する可能性がある。
            }
        }

        /// <summary>
        /// <c>Go to Brouser</c>コマンドの表示状態を設定します。
        /// </summary>
        private void SetCommandVisible()
        {
            var commandService = this.GetService<IMenuCommandService, OleMenuCommandService>();
            var menues = _config.MenuItems;

            for (int i = 0; i < menues.Count; i++)
            {
                var commandId = PkgCmdIDList.GetCommandId(i);
                var command = commandService.FindCommand(commandId) as OleMenuCommand;
                if (command != null)
                {
                    command.Visible = true;
                }
                else
                {
                    command = new OleMenuCommand(GoToBrowserCallback, commandId);
                    commandService.AddCommand(command);
                }

                command.Text = menues[i].Name;
            }

            for (int i = menues.Count; i < PkgCmdIDList.MAX_COMMAND_COUNT; i++)
            {
                var command = commandService.FindCommand(PkgCmdIDList.GetCommandId(i));
                if (command != null)
                {
                    command.Visible = false;
                }
            }
        }

        /// <summary>
        /// Visual Studio のメッセージボックスを表示します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="icon">表示するアイコン</param>
        private void ShowMessageBox(string message, OLEMSGICON icon)
        {
            var uiShell = this.GetService<SVsUIShell, IVsUIShell>();
            var clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                0,
                ref clsid,
                Resources.CommandIsNotExecutable,
                message,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                icon,
                0,        // false
                out result));
        }
    }
}