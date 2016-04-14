using System;

namespace qHUD.Framework.Helpers
{
    public static class StringExtend
    {
        public static bool Content(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}

