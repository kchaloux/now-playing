using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace NowPlaying.Converters
{
    /// <summary>
    /// Converts strings to and from numeric data.
    /// </summary>
    public class NumericTextConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as
        /// the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied. </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return targetType switch
                {
                    _ when targetType == typeof(byte) => System.Convert.ToByte(value),
                    _ when targetType == typeof(short) => System.Convert.ToInt16(value),
                    _ when targetType == typeof(int) => System.Convert.ToInt32(value),
                    _ when targetType == typeof(long) => System.Convert.ToInt64(value),
                    _ when targetType == typeof(float) => System.Convert.ToSingle(value),
                    _ when targetType == typeof(double) => System.Convert.ToDouble(value),
                    _ when targetType == typeof(decimal) => System.Convert.ToDecimal(value),
                    _ => DependencyProperty.UnsetValue,
                };
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
