namespace qHUD.Framework
{
    using System;
    public delegate IntPtr CCHookProc(IntPtr hWnd, ushort msg, int wParam, int lParam);
    public struct ChooseColor
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public int rgbResult;
        public IntPtr lpCustColors;
        public int Flags;
        public IntPtr lCustData;
        public CCHookProc lpfnHook;
        public IntPtr lpTemplateName;
    };
}