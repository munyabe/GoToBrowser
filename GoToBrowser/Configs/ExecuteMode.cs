namespace GoToBrowser.Configs
{
    /// <summary>
    /// コマンド実行時の動作を表します。
    /// </summary>
    public enum ExecuteMode
    {
        /// <summary>
        /// ブラウザで開きます。
        /// </summary>
        ShowBrowser = 0,

        /// <summary>
        /// クリップボードにコピーします。
        /// </summary>
        Copy = 1
    }
}