using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BatchImageProcessor.ViewModel.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            var parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            return parameterString == null ? DependencyProperty.UnsetValue : Enum.Parse(targetType, parameterString);
        }

        #endregion
    }
}