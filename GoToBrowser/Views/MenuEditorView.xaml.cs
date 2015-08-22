using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoToBrowser.Configs;

namespace GoToBrowser.Views
{
    /// <summary>
    /// MenuEditorView.xaml の相互作用ロジック
    /// </summary>
    public partial class MenuEditorView : Window
    {
        /// <summary>
        /// メニューの設定です。
        /// </summary>
        private CommandMenuItem _menuItem;

        /// <summary>
        /// 設定を適用されたときに発生します。
        /// </summary>
        public event EventHandler<CommandMenuItem> Applied;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public MenuEditorView()
            : this(null)
        {
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="menuItem">メニューの設定</param>
        public MenuEditorView(CommandMenuItem menuItem)
        {
            InitializeComponent();

            if (menuItem != null)
            {
                _menuItem = menuItem.Copy();
            }
            else
            {
                _menuItem = new CommandMenuItem();
                _saveButton.IsEnabled = false;
            }

            _menuItem.ErrorsChanged += (sender, e) =>
                _saveButton.IsEnabled = _menuItem.HasErrors == false;

            _macroList.ItemsSource = new UrlKeyFormat[]
            {
                new UrlKeyFormat(ConfigContents.FILE_NAME_KEY, "hoge"),
                new UrlKeyFormat(ConfigContents.FILE_PATH_KEY, "fuga"),
                new UrlKeyFormat(ConfigContents.LINE_NUMBER_KEY, "piyo"),
                new UrlKeyFormat(ConfigContents.SOLUTION_NAME_KEY, "boke")
            };

            DataContext = _menuItem;
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        /// <summary>
        /// <see cref="Applied"/>イベントを発生させます。
        /// </summary>
        protected virtual void OnApplied(CommandMenuItem item)
        {
            var handler = Applied;
            if (handler != null)
            {
                handler(this, item);
            }
        }

        /// <summary>
        /// 設定したメニューを適用します。
        /// </summary>
        private void Save(object sender, RoutedEventArgs e)
        {
            OnApplied(_menuItem);
            Close();
        }

        /// <summary>
        /// メニューの編集をキャンセルします。
        /// </summary>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// URLフォーマットにマクロを挿入します。
        /// </summary>
        private void InsertMacro(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem)sender;
            var urlKeyFormat = (UrlKeyFormat)item.Content;

            _urlFormat.Text = _urlFormat.Text.Insert(_urlFormat.SelectionStart, urlKeyFormat.Key);
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
                Abbreviation = string.Format(keyFormat, "A");
                Description = description;
            }
        }
    }
}
