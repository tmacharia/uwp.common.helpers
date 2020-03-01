using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Common;
using Windows.Data.Html;
using Windows.UI;

namespace UWP.Common.Helpers
{
    public static class Utils
    {
        public static byte ToByte(this int i) => (byte)i;
        public static string Hex(this byte b) => b.ToString("X2");
        public static string ToHex(this Color c) => $"#{c.R.Hex()}{c.G.Hex()}{c.B.Hex()}";
        public static Color HexToColor(this string hex)
        {
            hex = hex.TrimStart('#');

            Color col; 
            if (hex.Length == 6)
                col = Color.FromArgb(255, // hardcoded opaque
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber).ToByte(),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber).ToByte(),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber).ToByte());
            else 
                col = Color.FromArgb(
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber).ToByte(),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber).ToByte(),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber).ToByte(),
                            int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber).ToByte());
            return col;
        }
        public static string FromHtmlToText(this string html) => HtmlUtilities.ConvertToText(html);
        public static bool IsValidUrl(this string s) => Regex.IsMatch(s, UrlRegex);
        public static bool ContainsAnyUrl(this string text) => Regex.IsMatch(text, UrlRegex);
        public static string SanifUrl(this string badUrl)
        {
            if (!badUrl.StartsWithAnyOf("http", "www"))
            {
                int index = badUrl.IndexOf("http");
                if (index > -1)
                {
                    return badUrl.Substring(index);
                }
                else
                {
                    index = badUrl.IndexOf("www");
                    if (index > -1)
                    {
                        return badUrl.Substring(index);
                    }
                }
            }
            return badUrl;
        }

        public const string UrlRegex = @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})";

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> ts)
            => new ObservableCollection<T>(ts);
    }
}