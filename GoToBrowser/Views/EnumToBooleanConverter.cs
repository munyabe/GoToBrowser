using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GoToBrowser.Views
{
    /// <summary>
    /// 列挙値とブール値を変換するコンバーターです。
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// シングルトンのインスタンスです。
        /// </summary>
        public static readonly EnumToBooleanConverter Instance = new EnumToBooleanConverter();

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        private EnumToBooleanConverter()
        {
        }

        /// <summary>
        /// 列挙体の文字列をブール値に変換します。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.ToString() == parameter.ToString() : false;
        }

        /// <summary>
        /// ブール値を列挙体に変換します。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(targetType, parameter.ToString()) : DependencyProperty.UnsetValue;
        }
    }
}
