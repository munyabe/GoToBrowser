using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using GoToBrowser.Configs;
using GoToBrowser.Utils;
using GoToBrowser.Views;
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
        /// <c>Go to Brouser</c>コマンドの表示状態を設定します。
        /// </summary>
        private void SetCommandVisible()
        {
            var commandService = this.GetService<IMenuCommandService, OleMenuCommandService>();
            var menues = _config.MenuItems;

            for (int i = 0; i < menues.Count; i++)
            {
                var commandId = GoToBrowserCommand.GetCommandId(i);
                var command = commandService.FindCommand(commandId) as OleMenuCommand;
                if (command != null)
                {
                    command.Visible = true;
                }
                else
                {
                    GoToBrowserCommand.Initialize(this, i, _config);
                    command = GoToBrowserCommand.Instance.SourceCommand;
                }

                command.Text = menues[i].Name;
            }

            for (int i = menues.Count; i < GoToBrowserCommand.MaxCommandCount; i++)
            {
                var command = commandService.FindCommand(GoToBrowserCommand.GetCommandId(i));
                if (command != null)
                {
                    command.Visible = false;
                }
            }
        }
    }
}