// Copyright (c) 2017 Sidneys1
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
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