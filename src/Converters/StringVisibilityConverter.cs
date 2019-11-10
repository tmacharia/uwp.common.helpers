using System;
using Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class StringVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                return ((string)value).IsValid() ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}