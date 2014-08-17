using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BatchImageProcessor.ViewModel
{
	public class TypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter,
		  CultureInfo culture)
		{
			return value.GetType();
		}

		public object ConvertBack(object value, Type targetType, object parameter,
		  CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class EnumBooleanConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			if (Enum.IsDefined(value.GetType(), value) == false)
				return DependencyProperty.UnsetValue;

			object parameterValue = Enum.Parse(value.GetType(), parameterString);

			return parameterValue.Equals(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			return Enum.Parse(targetType, parameterString);
		}
		#endregion
	}

	public class EnumVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			if (Enum.IsDefined(value.GetType(), value) == false)
				return DependencyProperty.UnsetValue;

			object parameterValue = Enum.Parse(value.GetType(), parameterString);

			return parameterValue.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			return Enum.Parse(targetType, parameterString);
		}
		#endregion
	}

	public class EnumStringConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = value.ToString().Replace('_', ' ');

			return parameterString;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// TODO: write ConvertBack for EnumStringConverter
			throw new NotImplementedException();
		}
		#endregion
	}
}
