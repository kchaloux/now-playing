using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace NowPlaying.Converters
{
    /// <summary>
    /// Converts boolean values into visibilities.
    /// </summary>
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Gets or Sets whether Hidden should be used instead of Collapsed.
        /// </summary>
        public bool UseHidden { get; set; }

        /// <summary>
        /// Gets or Sets whether False values should be converted to Visible, rather than True values (default).
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided
        /// as the value of the target property for this markup extension.
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
            return value is bool boolValue
                ? Invert ^ boolValue
                    ? Visibility.Visible
                    : UseHidden
                        ? Visibility.Hidden
                        : Visibility.Collapsed
                : DependencyProperty.UnsetValue;
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
            throw new NotImplementedException();
        }
    }
}
