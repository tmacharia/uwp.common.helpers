using System;
using Common.Primitives;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class HumanizeFileSizeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                long n = System.Convert.ToInt64(value);
                int precision = 0;
                if(parameter != null)
                {
                    precision = System.Convert.ToInt32(parameter);
                }
                return n.HumanizeBytes(precision);
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}