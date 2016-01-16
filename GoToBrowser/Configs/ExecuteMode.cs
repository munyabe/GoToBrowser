namespace GoToBrowser.Configs
{
    /// <summary>
    /// コマンド実行時の動作を表します。
    /// </summary>
    /// <remarks>
    /// 設定ファイルに保存するため値を固定する必要があります。
    /// </remarks>
    public enum ExecuteMode
    {
        /// <summary>
        /// ブラウザで開きます。
        /// </summary>
        ShowBrowser = 0,

        /// <summary>
        /// クリップボードにコピーします。
        /// </summary>
        Copy = 1,

        /// <summary>
        /// エスケープせずにクリップボードにコピーします。
        /// </summary>
        UnescapedCopy = 2
    }
}