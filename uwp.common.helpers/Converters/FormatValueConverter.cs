using System;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class FormatValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                string txt = value.ToString();
                if (parameter == null)
                    return txt;
                else
                {
                    return parameter.ToString().Replace(":0", txt);
                }
            }
            if (parameter != null)
                return parameter.ToString();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}