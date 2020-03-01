using System;
using Common;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class ColorOpacityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is Color col)
            {
                int? n = parameter.ToInt();
                n = n.HasValue ? n : 100;
                return col.ToggleOpacity(n);
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
    class ColorValueConverters
    {
    }
}
