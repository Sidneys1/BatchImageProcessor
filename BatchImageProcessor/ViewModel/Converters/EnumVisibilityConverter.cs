using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace BatchImageProcessor.ViewModel.Converters
{
    public class EnumVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return null;

            var parameterString = parameter as string;

            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            var split =
                parameterString.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(
                        pstr =>
                            new
                            {
                                pVal = Enum.Parse(value.GetType(), pstr.Replace("!", "")),
                                invert = pstr.StartsWith("!")
                            }).ToArray();

            return split.Any(o => !Enum.IsDefined(value.GetType(), o.pVal))
                ? DependencyProperty.UnsetValue
                : (
                    split.Any(o => o.invert ? !o.pVal.Equals(value) : o.pVal.Equals(value))
                        ? Visibility.Visible
                        : Visibility.Collapsed
                    );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            return parameterString == null ? DependencyProperty.UnsetValue : Enum.Parse(targetType, parameterString);
        }

        #endregion
    }
}