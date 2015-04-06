using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BatchImageProcessor.ViewModel.Converters
{
    public class EnumStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = value.ToString().Replace('_', ' ');

            return parameterString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s == null)
                return DependencyProperty.UnsetValue;

            s = s.Replace(' ', '_');

            return Enum.Parse(targetType, s);
        }

        #endregion
    }
}