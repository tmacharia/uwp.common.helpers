using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class DoubleVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                return System.Convert.ToDouble(value) > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    class NumbersValueConverters
    {
    }
}