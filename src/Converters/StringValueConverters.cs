using System;
using Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class StringValueConverters
    {

    }
    public class StringValidVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is string s)
            {
                return s.IsValid() ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class StringInValidVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s)
            {
                return !s.IsValid() ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class PrefixStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is string s)
            {
                if (parameter != null)
                {
                    return $"{parameter} {s}";
                }
                return s;
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class CapitalizeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s)
            {
                return s.Capitalize();
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class ToUpperCaseValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s)
            {
                return s.ToUpper();
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class ToLowerCaseValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s)
            {
                return s.ToLower();
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}