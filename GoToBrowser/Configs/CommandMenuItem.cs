namespace GoToBrowser.Configs
{
    /// <summary>
    /// メニューの内容を保持するクラスです。
    /// </summary>
    public class CommandMenuItem
    {
        /// <summary>
        /// コマンド実行時の動作を取得または設定します。
        /// </summary>
        public ExecuteMode Mode { get; set; }

        /// <summary>
        /// メニュー名を取得または設定します。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL のフォーマットを取得または設定します。
        /// </summary>
        public string UrlFormat { get; set; }
    }
}