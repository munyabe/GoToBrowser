using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoToBrowser.Configs;
using GoToBrowser.Utils;

namespace GoToBrowser.Options
{
    /// <summary>
    /// <c>Go to Browser</c>の設定をするウィンドウです。
    /// </summary>
    public partial class ConfigWindow : Window
    {
        /// <summary>
        /// 設定を保持するインスタンスです。
        /// </summary>
        private ConfigContents _config;

        /// <summary>
        /// 設定を適用するときに発生します。
        /// </summary>
        public event EventHandler Apply;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="config">設定を保持するインスタンス</param>
        public ConfigWindow(ConfigContents config)
        {
            InitializeComponent();

            _config = config;
            urlFormat.Text = _config.UrlFormat;

            Title = Properties.Resources.PluginName;
            DataContext = new UrlKeyFormat[]
            {
                new UrlKeyFormat(ConfigContents.FILE_NAME_KEY, Properties.Resources.FileNameKeyDescription),
                new UrlKeyFormat(ConfigContents.FILE_PATH_KEY, Properties.Resources.FilePathKeyDescription),
                new UrlKeyFormat(ConfigContents.LINE_NUMBER_KEY, Properties.Resources.LineNumberKeyDescription),
                new UrlKeyFormat(ConfigContents.SOLUTION_NAME_KEY, Properties.Resources.SolutionNameKeyDescription)
            };

            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        /// <summary>
        /// <see cref="Apply"/>イベントを発生させます。
        /// </summary>
        protected virtual void OnApply()
        {
            var handler = Apply;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// OKボタンをクリックしたときの処理です。
        /// </summary>
        private void OKClick(object sender, RoutedEventArgs e)
        {
            _config.UrlFormat = urlFormat.Text;

            OnApply();
            Close();
        }

        /// <summary>
        /// キャンセルボタンをクリックしたときの処理です。
        /// </summary>
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// URLフォーマットリストの項目がダブルクリックされたときの処理です。
        /// </summary>
        private void UrlFormatKeyDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            var urlKeyFormat = item.Content as UrlKeyFormat;

            urlFormat.Text += urlKeyFormat.Key;
        }

        /// <summary>
        /// URLフォーマットのキー情報を保持するクラスです。
        /// </summary>
        private class UrlKeyFormat
        {
            public string Key { get; private set; }
            public string Abbreviation { get; private set; }
            public string Description { get; private set; }

            /// <summary>
            /// インスタンスを初期化します。
            /// </summary>
            public UrlKeyFormat(string key, string description)
            {
                const string keyFormat = "{{{0}}}";
                Key = string.Format(keyFormat, key);
                Abbreviation = string.Format(keyFormat, StringUtil.GetUpperCases(key));
                Description = description;
            }
        }
    }
}
