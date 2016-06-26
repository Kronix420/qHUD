namespace qHUD.Framework.InputHooks.Structures
{
    using System.Drawing;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseLLHookStruct
    {
        public Point Point;
        public int MouseData;
        public int Flags;
        public int Time;
        public int ExtraInfo;
    }
}