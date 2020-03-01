using System;
using Common.Structs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class NullOrMinDateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => DateTimeConverterExts.IsNullOrMin(value);
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class NullOrMinDateVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => DateTimeConverterExts.IsNullOrMin(value) ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class ToDateTimeMomentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dto)
            {
                return dto.ToDateTimeMoment();
            }
            else if (value is DateTime dt)
            {
                return dt.ToDateTimeMoment();
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public class DateToMomentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTime dt)
            {
                return dt.ToMoment();
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    public static class DateTimeConverterExts
    {
        public static bool IsNullOrMin(object obj)
        {
            var dt = CastToDateTime(obj);
            if (dt.HasValue)
                return dt.Value > DateTime.MinValue;
            return true;
        }
        public static DateTime? CastToDateTime(object obj)
        {
            if(obj != null) {
                try {
                    return (DateTime?)obj;
                }
                catch (Exception)
                { }
            }
            return null;
        }
    }
}