using System;
using Common.Structs;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class DateToMomentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                DateTime? dt = null;
                if (value is DateTime?)
                    dt = ((DateTime?)value);
                else if (value is DateTime)
                    dt = (DateTime)value;

                if(parameter == null)
                    return dt.Value.ToMoment();
                else
                {
                    return parameter.ToString().Replace(":0", dt.Value.ToMoment());
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}