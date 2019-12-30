using Common;
using Windows.UI;
using System.Linq;
using Windows.UI.Xaml.Media;
using System.Collections.Generic;

namespace UWP.Common.Helpers
{
    public static class ColorExts
    {
        public static double Average(this Color col) => (col.A + col.R + col.G + col.B) / 4;
        public static Color GetDominantColor(this IEnumerable<Color> colors)
            => colors.GetDominantColorAsHex().HexToColor();
        public static string GetDominantColorAsHex(this IEnumerable<Color> colors)
        {
            Dictionary<string, int> store = new Dictionary<string, int>();

            colors.ForEach(col =>
            {
                string hex = col.ToHex();
                if (store.ContainsKey(hex))
                    store[hex]++;
                else
                    store.Add(hex, 1);
            });
            return store.OrderByDescending(x => x.Value).FirstOrDefault().Key;
        }
        public static Brush ToBrush(this Color col) => new SolidColorBrush(col);
        public static SolidColorBrush ToSolidColorBrush(this Color col) => new SolidColorBrush(col);
        public static SolidColorBrush ToggleOpacity(this SolidColorBrush brush, int? opacity = 100)
        {
            brush.Color = brush.Color.ToggleOpacity(opacity);
            return brush;
        }
        public static Color ToggleOpacity(this Color col, int? opacity = 100)
        {
            int alpha = 255;
            if (opacity.HasValue)
            {
                alpha = opacity.Value * 255 / 100;
            }

            return Color.FromArgb((byte)alpha, col.R, col.G, col.B);
        }
    }
}