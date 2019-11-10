using System;
using System.Text;
using Common;
using Windows.Data.Html;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class HtmlValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            string[] lines = value.ToString().Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].IsValid())
                    sb.AppendLine();
                else
                    sb.AppendLine(HtmlUtilities.ConvertToText(lines[i]));
            }
            return sb.ToString().Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}