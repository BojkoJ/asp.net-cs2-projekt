using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BOJ0043_App
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter != null && parameter.ToString()?.ToLower() == "invert";
            bool isVisible = value is bool b && b;
            if (invert)
                isVisible = !isVisible;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
