using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using GoToBrowser.Options;
using GoToBrowser.Properties;
using GoToBrowser.Utils;
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
    [InstalledProductRegistration("#110", "#112", "1.20", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidGoToBrowserPkgString)]
    public sealed partial class GoToBrowserPackage : Package
    {
        /// <summary>
        /// <c>Go to Brouser</c>の設定です。
        /// </summary>
        private GeneralConfig _config = new GeneralConfig();

        /// <summary>
        /// <c>Go to Brouser</c>を実行するコマンドです。
        /// </summary>
        private MenuCommand _goToBrowserCommand;

        /// <summary>
        /// パッケージを初期化します。
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var commandService = this.GetService<IMenuCommandService, OleMenuCommandService>();
            var goToBrowserCommandId = new CommandID(GuidList.guidGoToBrowserCmdSet, (int)PkgCmdIDList.goToBrowserCommand);
            _goToBrowserCommand = new MenuCommand(GoToBrowserCallback, goToBrowserCommandId);
            commandService.AddCommand(_goToBrowserCommand);

            var settingCommandID = new CommandID(GuidList.guidGoToBrowserCmdSet, (int)PkgCmdIDList.configureCommand);
            var configureCommand = new OleMenuCommand(ConfigureCallback, settingCommandID);
            commandService.AddCommand(configureCommand);

            var solution = this.GetService<SVsSolution, IVsSolution>();
            uint solutionEventCoockie;
            solution.AdviseSolutionEvents(this, out solutionEventCoockie);
        }

        /// <summary>
        /// .suo ファイルを読み込む際の処理です。
        /// </summary>
        /// <remarks><c>.suo</c>ファイルに<paramref name="key"/>が存在しない場合は呼び出されません。</remarks>
        protected override void OnLoadOptions(string key, Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                _config.UrlFormat = reader.ReadString();
            }
        }

        /// <summary>
        /// .suo ファイルを保存する際の処理です。
        /// </summary>
        protected override void OnSaveOptions(string key, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(_config.UrlFormat);
            }

            base.OnSaveOptions(key, stream);
        }

        /// <summary>
        /// ドキュメントのカーソル位置の行数を取得します。
        /// </summary>
        private int GetCurrentLineNumber(Document document)
        {
            var textDocument = document.Object("TextDocument") as TextDocument;
            return textDocument != null ? textDocument.Selection.ActivePoint.Line : 0;
        }

        /// <summary>
        /// <c>Go to Browser</c>コマンドを実行したときの処理です。
        /// </summary>
        private void GoToBrowserCallback(object sender, EventArgs e)
        {
            try
            {
                ExecuteGoToBrowser();
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
            var window = new ConfigWindow(_config);
            window.Apply += (applySender, applyArgs) =>
            {
                var persistence = this.GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
                persistence.SavePackageUserOpts(this, GeneralConfig.URL_FORMAT_SUO_KEY);

                SetCommandVisible();
            };

            window.ShowDialog();
        }

        /// <summary>
        /// <c>Go to Browser</c>コマンドを実行します。
        /// </summary>
        private void ExecuteGoToBrowser()
        {
            var dte = this.GetService<DTE>();
            var solutionPath = Path.GetDirectoryName(dte.Solution.FullName);
            var document = dte.ActiveDocument;

            var values = new Dictionary<string, string>();
            Action<string, string> addValue = (keyName, value) =>
            {
                values[keyName] = value;
                values[StringUtil.GetUpperCases(keyName)] = value;
            };

            var filePath = document.FullName.Replace(solutionPath, string.Empty).Replace("\\", "/");
            addValue(GeneralConfig.FILE_NAME_KEY, Path.GetFileName(filePath));
            addValue(GeneralConfig.FILE_PATH_KEY, filePath);
            addValue(GeneralConfig.LINE_NUMBER_KEY, GetCurrentLineNumber(document).ToString());
            addValue(GeneralConfig.SOLUTION_NAME_KEY, _config.SolutionName);

            var resultUri = Uri.EscapeUriString(StringUtil.Format(_config.UrlFormat, values));
            dte.ExecuteCommand("navigate", string.Format("{0} /ext", resultUri));
        }

        /// <summary>
        /// <c>Go to Brouser</c>コマンドの表示状態を設定します。
        /// </summary>
        private void SetCommandVisible()
        {
            _goToBrowserCommand.Visible = string.IsNullOrWhiteSpace(_config.UrlFormat) == false;
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