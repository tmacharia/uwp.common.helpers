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
    public class DateToMomentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = DateTimeConverterExts.CastToDateTime(value);
            if (dt.HasValue)
                return parameter == null ? dt.Value.ToMoment() : parameter.ToString().Replace(":0", dt.Value.ToMoment());
            return "";
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