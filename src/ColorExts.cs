using System.Collections.Generic;
using System.Linq;
using Common;
using Windows.UI;

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
    }
}