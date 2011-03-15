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
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(GeneralOption), "Go to Browser", "General", 101, 106, true)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidGoToBrowserPkgString)]
    public sealed partial class GoToBrowserPackage : Package
    {
        /// <summary>
        /// URLフォーマットを<c>.suo</c>ファイルに保存する際のキーです。
        /// </summary>
        private const string URL_FORMAT_SUO_KEY = "GoToBrouser.URLFormat";

        /// <summary>
        /// <c>Go to Brouser</c>を実行するコマンドです。
        /// </summary>
        private MenuCommand _goToBrowserCommand;

        /// <summary>
        /// オプションの設定です。
        /// </summary>
        private GeneralOption _option;

        /// <summary>
        /// パッケージを初期化します。
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _option = GetDialogPage<GeneralOption>();
            _option.SettingsSaved += (sender, e) =>
            {
                var persistence = this.GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
                persistence.SavePackageUserOpts(this, URL_FORMAT_SUO_KEY);

                SetCommandVisible();
            };

            var commandService = this.GetService<IMenuCommandService, OleMenuCommandService>();
            var goToBrowserCommandId = new CommandID(GuidList.guidGoToBrowserCmdSet, (int)PkgCmdIDList.goToBrowserCommand);
            _goToBrowserCommand = new MenuCommand(GoToBrowserCallback, goToBrowserCommandId);
            commandService.AddCommand(_goToBrowserCommand);

            var solution = this.GetService<SVsSolution, IVsSolution>();
            uint solutionEventCoockie;
            solution.AdviseSolutionEvents(this, out solutionEventCoockie);
        }

        /// <summary>
        /// <c>.suo</c>ファイルを読み込む際の処理です。
        /// </summary>
        protected override void OnLoadOptions(string key, Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                _option.UrlFormat = reader.ReadString();
            }
        }

        /// <summary>
        /// <c>.suo</c>ファイルを保存する際の処理です。
        /// </summary>
        protected override void OnSaveOptions(string key, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(_option.UrlFormat);
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
        /// オプションの設定を取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetDialogPage<T>() where T : DialogPage
        {
            var dialogPage = GetDialogPage(typeof(T)) as T;
            if (dialogPage == null)
            {
                throw new InvalidOperationException(string.Format("The dialog page [{0}] is not found.", typeof(T).Name));
            }

            return dialogPage;
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
        /// <c>Go to Browser</c>コマンドを実行します。
        /// </summary>
        private void ExecuteGoToBrowser()
        {
            var dte = this.GetService<DTE>();
            string solutionPath = Path.GetDirectoryName(dte.Solution.FullName);
            var document = dte.ActiveDocument;

            var values = new Dictionary<string, string>();
            Action<string, string> addValue = (keyName, value) =>
            {
                values[keyName] = value;
                values[StringUtil.GetUpperCases(keyName)] = value;
            };

            var filePath = document.FullName.Replace(solutionPath, string.Empty);
            addValue(GeneralOption.FILE_NAME_KEY, Path.GetFileName(filePath));
            addValue(GeneralOption.FILE_PATH_KEY, filePath);
            addValue(GeneralOption.LINE_NUMBER_KEY, GetCurrentLineNumber(document).ToString());
            addValue(GeneralOption.SOLUTION_NAME_KEY, _option.SolutionName);

            var resultUri = StringUtil.Format(_option.UrlFormat, values);
            dte.ExecuteCommand("navigate", string.Format("{0} /new /ext", resultUri));
        }

        /// <summary>
        /// <c>Go to Brouser</c>コマンドの表示状態を設定します。
        /// </summary>
        private void SetCommandVisible()
        {
            _goToBrowserCommand.Visible = string.IsNullOrWhiteSpace(_option.UrlFormat) == false;
        }

        /// <summary>
        /// <c>Visual Studio</c>のメッセージボックスを表示します。
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