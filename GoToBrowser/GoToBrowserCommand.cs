using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using GoToBrowser.Configs;
using GoToBrowser.Utils;
using Microsoft.VisualStudio.Shell;

namespace GoToBrowser
{
    /// <summary>
    /// 指定のリンクをブラウザで開く、もしくはコピーするコマンドです。
    /// </summary>
    internal sealed class GoToBrowserCommand : CommandBase
    {
        /// <summary>
        /// 1つ目のコマンドのIdです。
        /// </summary>
        public const int CommandId = 0x0101;

        /// <summary>
        /// 設定できるコマンド数の上限値です。
        /// </summary>
        public const int MaxCommandCount = 8;

        /// <summary>
        /// コマンドの設定です。
        /// </summary>
        private readonly ConfigContents _config;

        /// <summary>
        /// シングルトンのインスタンスを取得します。
        /// </summary>
        public static GoToBrowserCommand Instance { get; private set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="package">コマンドを提供するパッケージ</param>
        /// <param name="index">コマンドのインデックス</param>
        /// <param name="config">コマンドの設定</param>
        private GoToBrowserCommand(Package package, int index, ConfigContents config) :
            base(package, CommandId + index, GuidList.guidGoToBrowserCmdSet)
        {
            _config = config;
        }

        /// <summary>
        /// コマンドとグループのIdを含んだ、コマンドの識別子を取得します。
        /// </summary>
        /// <param name="index">コマンドのインデックス</param>
        /// <returns>コマンドの識別子</returns>
        public static CommandID GetCommandId(int index)
        {
            GuardCommandIndex(index);
            return new CommandID(GuidList.guidGoToBrowserCmdSet, CommandId + index);
        }

        /// <summary>
        /// シングルトンのインスタンスを初期化します。
        /// </summary>
        /// <param name="package">コマンドを提供するパッケージ</param>
        /// <param name="index">コマンドのインデックス</param>
        /// <param name="config">コマンドの設定</param>
        public static void Initialize(Package package, int index, ConfigContents config)
        {
            GuardCommandIndex(index);
            Instance = new GoToBrowserCommand(package, index, config);
        }

        /// <inheritdoc />
        protected override void Execute(object sender, EventArgs e)
        {
            var command = (MenuCommand)sender;
            var item = _config.MenuItems[command.CommandID.ID - CommandId];

            var dte = ServiceProvider.GetService<DTE>();
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
                var project = solution.Projects.OfType<Project>().FirstOrDefault();
                if (project != null)
                {
                    result = project.FullName;
                }
            }

            return result;
        }

        /// <summary>
        /// コマンドのインデックスが正しい範囲にあることを示します。
        /// </summary>
        private static void GuardCommandIndex(int index)
        {
            if (index < 0 || MaxCommandCount - 1 < index)
            {
                throw new ArgumentOutOfRangeException(string.Format("The int argument [{0}] must be positive and less than {1}.", index, MaxCommandCount));
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
    }
}
