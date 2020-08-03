using System;
using Common.Structs;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    class TimespanValueConverters
    {
    }
    public class ToTimeSpanMomentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan span)
            {
                return span.FormatTimespan();
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}