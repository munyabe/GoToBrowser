using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GoToBrowser.Utils;

namespace GoToBrowser.Configs
{
    /// <summary>
    /// メニューの内容を保持するクラスです。
    /// </summary>
    public class CommandMenuItem : INotifyDataErrorInfo
    {
        /// <summary>
        /// エンティティ全体の検証エラーです。
        /// </summary>
        private readonly IDictionary<string, IList<string>> _errors = new Dictionary<string, IList<string>>();

        /// <summary>
        /// エンティティの値を一度も検証していないことを表します。
        /// </summary>
        private bool _isInitialValidate;

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <inheritdoc />
        public bool HasErrors
        {
            get { return _errors.Count != 0; }
        }

        /// <summary>
        /// コマンド実行時の動作を取得または設定します。
        /// </summary>
        public ExecuteMode Mode { get; set; }

        private string _name;
        /// <summary>
        /// メニュー名を取得または設定します。
        /// </summary>
        [Required]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    ValidateProperty("Name", value);
                }
            }
        }

        private string _urlFormat;
        /// <summary>
        /// URL のフォーマットを取得または設定します。
        /// </summary>
        [Required]
        public string UrlFormat
        {
            get { return _urlFormat; }
            set
            {
                if (_urlFormat != value)
                {
                    _urlFormat = value;
                    ValidateProperty("UrlFormat", value);
                }
            }
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <remarks>
        /// 次回にプロパティを1つでも設定すると、初回はエンティティ全体が検証されます。
        /// </remarks>
        public CommandMenuItem()
        {
            _isInitialValidate = true;
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public CommandMenuItem(string name, string urlFormat, ExecuteMode mode)
        {
            Name = name;
            UrlFormat = urlFormat;
            Mode = mode;
        }

        /// <summary>
        /// インスタンスをコピーします。
        /// </summary>
        public CommandMenuItem Copy()
        {
            return new CommandMenuItem(Name, UrlFormat, Mode);
        }

        /// <inheritdoc />
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values.SelectMany(x => x);
            }

            IList<string> errorMessages;
            return _errors.TryGetValue(propertyName, out errorMessages) ? errorMessages : null;
        }

        /// <summary>
        /// <see cref="ErrorsChanged"/>イベントを発生させます。
        /// </summary>
        /// <param name="propertyName">エラーのあるプロパティ名</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// エンティティ全体の値を検証します。
        /// </summary>
        private void ValidateObject()
        {
            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateObject(this, new ValidationContext(this), validationResults) == false)
            {
                validationResults
                    .SelectMany(x => x.MemberNames.Select(name => new { MemberName = name, ErrorMessage = x.ErrorMessage }))
                    .GroupBy(x => x.MemberName, x => x.ErrorMessage)
                    .ForEach(x => _errors[x.Key] = x.ToList());
            }
        }

        /// <summary>
        /// 指定のプロパティを検証します。
        /// </summary>
        /// <param name="propertyName">検証するプロパティ名</param>
        /// <param name="value">検証するプロパティの値</param>
        private void ValidateProperty(string propertyName, object value)
        {
            if (_isInitialValidate)
            {
                _isInitialValidate = false;
                ValidateObject();
                return;
            }

            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateProperty(value, new ValidationContext(this) { MemberName = propertyName }, validationResults))
            {
                if (_errors.Remove(propertyName))
                {
                    OnErrorsChanged(propertyName);
                }
            }
            else
            {
                _errors[propertyName] = validationResults.Select(x => x.ErrorMessage).ToList();
                OnErrorsChanged(propertyName);
            }
        }
    }
}