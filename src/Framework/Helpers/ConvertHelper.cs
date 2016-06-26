using System.Collections.Generic;

namespace qHUD.Framework.Helpers
{
    using System.Globalization;
    using SharpDX;
    public static class ConvertHelper
    {
        public static Color ToBGRAColor(this string value)
        {
            uint bgra;
            return uint.TryParse(value, NumberStyles.HexNumber, null, out bgra)
                ? Color.FromBgra(bgra) : Color.Black;
        }

        public static Color? ConfigColorValueExtractor(this string[] line, int index)
        {
            return IsNotNull(line, index) ? (Color?)line[index].ToBGRAColor() : null;
        }

        public static string ConfigValueExtractor(this string[] line, int index)
        {
            return IsNotNull(line, index) ? line[index] : null;
        }

        private static bool IsNotNull(IReadOnlyList<string> line, int index)
        {
            return line.Count > index && !string.IsNullOrEmpty(line[index]);
        }
    }
}