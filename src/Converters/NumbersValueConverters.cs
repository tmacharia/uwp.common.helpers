using System;
using Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class IntVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                return System.Convert.ToInt32(value) > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class IsZeroVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var n = value.ToInt();
                return n.HasValue ? n.Value == 0 ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class IntToBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                int n = System.Convert.ToInt32(value);
                return n == 1 || (n == 0 && false);
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
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
    public class DoubleToBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                double n = System.Convert.ToDouble(value);
                return n == 1 || (n == 0 && false);
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    
    class NumbersValueConverters
    {
    }
}