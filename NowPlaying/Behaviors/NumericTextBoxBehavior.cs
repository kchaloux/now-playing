using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace NowPlaying.Behaviors
{
    /// <summary>
    /// Prevent non-numeric data from being entered into a text box.
    /// </summary>
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Gets or Sets whether decimal values are allowed. Only integer
        /// values will be allowed if this is false.
        /// </summary>
        public bool IsDecimal { get; set; }

        /// <summary>
        /// Gets or Sets the minimum numeric value to allow in the text box.
        /// </summary>
        public double Minimum { get; set; } = double.MinValue;

        /// <summary>
        /// Gets or Sets the minimum numeric value to allow in the text box.
        /// </summary>
        public double Maximum { get; set; } = double.MaxValue;

        private static readonly Regex ValidNumberRegex = new Regex(
            @"(-|[0-9]|\.|[eE])+",
            RegexOptions.Compiled);

        private string _lastValidText = "";

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectOnPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectOnPreviewKeyDown;
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            AssociatedObject.LostFocus += AssociatedObjectOnLostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectOnPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectOnPreviewKeyDown;
            AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
            AssociatedObject.LostFocus -= AssociatedObjectOnLostFocus;
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_lastValidText == "")
            {
                _lastValidText = AssociatedObject.Text;
            }
        }

        private void AssociatedObjectOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!ValidNumberRegex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void AssociatedObjectOnLostFocus(object sender, RoutedEventArgs e)
        {
            ValidateAndUpdateSource();
        }

        private void AssociatedObjectOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            ValidateAndUpdateSource();
        }

        private void ValidateAndUpdateSource()
        {
            try
            {
                var numericValue = Convert.ToDouble(AssociatedObject.Text);
                if (!IsDecimal)
                {
                    numericValue = (int)numericValue;
                }

                if (numericValue >= Minimum && numericValue <= Maximum)
                {
                    _lastValidText = AssociatedObject.Text;
                }
                else
                {
                    var clampedValue = Math.Min(Maximum, Math.Max(Minimum, numericValue));
                    if (!IsDecimal)
                    {
                        clampedValue = (int)clampedValue;
                    }
                    AssociatedObject.Text = clampedValue.ToString(CultureInfo.CurrentCulture);
                    _lastValidText = AssociatedObject.Text;
                }
            }
            catch
            {
                AssociatedObject.Text = _lastValidText;
            }
            var binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }
}
