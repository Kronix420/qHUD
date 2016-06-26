namespace qHUD.Framework.Helpers
{
    using System;
    public static class ActionHelper
    {
        public static void SafeInvoke<T>(this Action<T> action, T parameter)
        {
            action?.Invoke(parameter);
        }

        public static void SafeInvoke(this Action action)
        {
            action?.Invoke();
        }
    }
}