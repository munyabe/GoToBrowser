namespace GoToBrowser.Options
{
    /// <summary>
    /// 設定情報を保持するクラスです。
    /// </summary>
    public class GeneralConfig
    {
        /// <summary>
        /// URLフォーマットを .suo ファイルに保存する際のキーです。
        /// </summary>
        public const string URL_FORMAT_SUO_KEY = "GoToBrouser.UrlFormat";

        public const string FILE_NAME_KEY = "FileName";
        public const string FILE_PATH_KEY = "FilePath";
        public const string LINE_NUMBER_KEY = "LineNumber";
        public const string SOLUTION_NAME_KEY = "SolutionName";

        /// <summary>
        /// URLフォーマットを取得または設定します。
        /// </summary>
        public string UrlFormat { get; set; }

        /// <summary>
        /// 現在のソリューション名を取得または設定します。
        /// </summary>
        public string SolutionName { get; set; }
    }
}
