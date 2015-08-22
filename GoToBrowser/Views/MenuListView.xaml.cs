using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GoToBrowser.Configs;

namespace GoToBrowser.Views
{
    /// <summary>
    /// MenuListView.xaml の相互作用ロジック
    /// </summary>
    public partial class MenuListView : Window
    {
        /// <summary>
        /// 設定したメニューの一覧です。
        /// </summary>
        private ObservableCollection<CommandMenuItem> _menuItems;

        /// <summary>
        /// 設定を適用されたときに発生します。
        /// </summary>
        public event EventHandler<IList<CommandMenuItem>> Applied;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public MenuListView(IList<CommandMenuItem> menuItems)
        {
            InitializeComponent();

            _menuItems = new ObservableCollection<CommandMenuItem>(menuItems);
            DataContext = _menuItems;

            SetCanExecute();
            _menuItems.CollectionChanged += (sender, e) => SetCanExecute();
            _menuList.SelectionChanged += (sender, e) => SetCanExecute();
            _moveUpButton.Click += (sender, e) => SetMoveCommandCanExecute();
            _moveDownButton.Click += (sender, e) => SetMoveCommandCanExecute();

            if (_menuItems.Count != 0)
            {
                _menuList.SelectedIndex = 0;
            }
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        /// <summary>
        /// <see cref="Applied"/>イベントを発生させます。
        /// </summary>
        protected virtual void OnApplied(IList<CommandMenuItem> items)
        {
            var handler = Applied;
            if (handler != null)
            {
                handler(this, items);
            }
        }

        /// <summary>
        /// メニューを追加します。
        /// </summary>
        private void AddMenu(object sender, RoutedEventArgs e)
        {
            var window = new MenuEditorView();
            window.Applied += (appliedSender, appliedArgs) =>
            {
                _menuItems.Add(appliedArgs);
                _menuList.SelectedIndex = _menuItems.Count - 1;
            };

            window.ShowDialog();
        }

        /// <summary>
        /// メニューを編集します。
        /// </summary>
        private void EditMenu(object sender, RoutedEventArgs e)
        {
            var selectedItem = _menuList.SelectedItem as CommandMenuItem;
            if (selectedItem == null)
            {
                return;
            }

            var window = new MenuEditorView(selectedItem);
            window.Applied += (appliedSender, appliedArgs) =>
            {
                var index = _menuList.SelectedIndex;
                _menuItems.RemoveAt(index);
                _menuItems.Insert(index, appliedArgs);
            };

            window.ShowDialog();
        }

        /// <summary>
        /// メニューを削除します。
        /// </summary>
        private void RemoveMenu(object sender, RoutedEventArgs e)
        {
            if (IsSelectedMenu())
            {
                _menuItems.RemoveAt(_menuList.SelectedIndex);
            }
        }

        /// <summary>
        /// メニューを上へ移動します。
        /// </summary>
        private void MoveUp(object sender, RoutedEventArgs e)
        {
            var index = _menuList.SelectedIndex;
            if (CanExecuteMoveUp(index))
            {
                _menuItems.Move(index, index - 1);
            }
        }

        /// <summary>
        /// メニューを下へ移動します。
        /// </summary>
        private void MoveDown(object sender, RoutedEventArgs e)
        {
            var index = _menuList.SelectedIndex;
            if (CanExecuteMoveDown(index))
            {
                _menuItems.Move(index, index + 1);
            }
        }

        /// <summary>
        /// 設定したメニューを適用します。
        /// </summary>
        private void Save(object sender, RoutedEventArgs e)
        {
            OnApplied((IList<CommandMenuItem>)DataContext);
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
        /// メニューを上へ移動コマンドが実行可能かどうかを判定します。
        /// </summary>
        private bool CanExecuteMoveUp(int selectedIndex)
        {
            return 0 < selectedIndex;
        }

        /// <summary>
        /// メニューを下へ移動コマンドが実行可能かどうかを判定します。
        /// </summary>
        private bool CanExecuteMoveDown(int selectedIndex)
        {
            return 0 <= selectedIndex && selectedIndex < _menuItems.Count - 1;
        }

        /// <summary>
        /// 一覧からメニューが選択されているかどうかを判定します。
        /// </summary>
        private bool IsSelectedMenu()
        {
            return 0 <= _menuList.SelectedIndex;
        }

        /// <summary>
        /// コマンドの活性制御を設定します。
        /// </summary>
        private void SetCanExecute()
        {
            _addButton.IsEnabled = _menuItems.Count < 8;

            var isSelected = IsSelectedMenu();
            _editButton.IsEnabled = isSelected;
            _removeButton.IsEnabled = isSelected;

            SetMoveCommandCanExecute();
        }

        /// <summary>
        /// メニューの移動コマンドの活性制御を設定します。
        /// </summary>
        private void SetMoveCommandCanExecute()
        {
            var selectedIndex = _menuList.SelectedIndex;
            _moveUpButton.IsEnabled = CanExecuteMoveUp(selectedIndex);
            _moveDownButton.IsEnabled = CanExecuteMoveDown(selectedIndex);
        }
    }
}