namespace qHUD.Framework.Helpers
{
    using System;
    public static class StringExtend
    {
        public static bool Content(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}

