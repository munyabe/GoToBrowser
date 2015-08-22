using System;
using System.Collections.Generic;
using System.IO;
using GoToBrowser.Utils;

namespace GoToBrowser.Configs
{
    /// <summary>
    /// 設定内容を保持するクラスです。
    /// </summary>
    public class ConfigContents
    {
        /// <summary>
        /// 設定内容を .suo ファイルに保存する際のキーです。
        /// </summary>
        public const string CONFIG_SUO_KEY = "GoToBrouser.UrlFormat";

        /// <summary>
        /// URLフォーマットでファイル名に置換されるキーです。
        /// </summary>
        public const string FILE_NAME_KEY = "FileName";

        /// <summary>
        /// URLフォーマットでファイルの相対パスに置換されるキーです。
        /// </summary>
        public const string FILE_PATH_KEY = "FilePath";

        /// <summary>
        /// URLフォーマットでファイルの行数に置換されるキーです。
        /// </summary>
        public const string LINE_NUMBER_KEY = "LineNumber";

        /// <summary>
        /// URLフォーマットでソリューション名に置換されるキーです。
        /// </summary>
        public const string SOLUTION_NAME_KEY = "SolutionName";

        /// <summary>
        /// メニューの一覧を取得または設定します。
        /// </summary>
        public IList<CommandMenuItem> MenuItems { get; set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public ConfigContents()
        {
            MenuItems = new List<CommandMenuItem>();
        }

        /// <summary>
        /// URL フォーマットに適用できるマクロの一覧を作成します。
        /// </summary>
        /// <param name="solutionFullName">ソリューションファイルの絶対パス/param>
        /// <param name="fileFullName">現在開いているファイルの絶対パス</param>
        /// <param name="lineNumber">現在のカーソル位置の行数</param>
        /// <returns>マクロの一覧</returns>
        public static IDictionary<string, string> CreateMacros(string solutionFullName, string fileFullName, int lineNumber)
        {
            var result = new Dictionary<string, string>();
            Action<string, string> addValue = (key, value) =>
            {
                result[key] = value;
                result[StringUtil.GetUpperCases(key)] = value;
            };

            var solutionDirectory = Path.GetDirectoryName(solutionFullName);
            var filePath = fileFullName.Replace(solutionDirectory, string.Empty).Replace("\\", "/");
            addValue(ConfigContents.FILE_NAME_KEY, Path.GetFileName(filePath));
            addValue(ConfigContents.FILE_PATH_KEY, filePath);
            addValue(ConfigContents.LINE_NUMBER_KEY, lineNumber.ToString());
            addValue(ConfigContents.SOLUTION_NAME_KEY, Path.GetFileNameWithoutExtension(solutionFullName));

            return result;
        }
    }
}
