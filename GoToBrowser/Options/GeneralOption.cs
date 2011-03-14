using System;
using System.ComponentModel;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using GoToBrowser.Utils;

namespace GoToBrowser.Options
{
    public class GeneralOption : DialogPage
    {
        private GeneralOptionPage _dialog = new GeneralOptionPage();

        public const string FILE_NAME_KEY = "FileName";
        public const string FILE_PATH_KEY = "FilePath";
        public const string LINE_NUMBER_KEY = "LineNumber";
        public const string SOLUTION_NAME_KEY = "SolutionName";

        public event EventHandler SettingsSaved;

        public string SolutionName
        {
            get { return _dialog.SolutionName; }
            set { _dialog.SolutionName = value; }
        }

        [Category("General")]
        public string UrlFormat
        {
            get { return _dialog.UrlFormat; }
            set { _dialog.UrlFormat = value; }
        }

        protected override IWin32Window Window
        {
            get { return _dialog; }
        }

        //public override void LoadSettingsFromStorage()
        //{
        //    // MEMO : レジストリを使わないため、ロードしない
        //    //base.LoadSettingsFromStorage();
        //}

        //public override void SaveSettingsToStorage()
        //{
        //    if (string.IsNullOrWhiteSpace(SolutionName) == false)
        //    {
        //        UrlFormat = _dialog.UrlFormat;
        //        OnSettingsSaved();
        //    }
        //    //base.SaveSettingsToStorage();
        //}

        /// <summary>
        /// オプション画面でOKボタンをクリックしたときの処理です。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (string.IsNullOrWhiteSpace(SolutionName) == false)
            {
                UrlFormat = _dialog.UrlFormat;
                OnSettingsSaved();
            }
        }

        /// <summary>
        /// <see cref="SettingsSaved"/>イベントを発生させます。
        /// </summary>
        protected virtual void OnSettingsSaved()
        {
            var handler = SettingsSaved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // TODO : 不要
        /// <summary>
        /// IDEのインスタンスを取得します。
        /// </summary>
        /// <param name="dialog">オプションダイアログ</param>
        /// <returns>IDEのインスタンス</returns>
        private static DTE GetIDE(DialogPage dialog)
        {
            if (dialog != null && dialog.Site != null)
            {
                return dialog.Site.GetService<DTE>();
            }
            else
            {
                return null;
            }
        }
    }
}