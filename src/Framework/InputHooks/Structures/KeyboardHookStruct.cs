namespace qHUD.Framework.InputHooks.Structures
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyboardHookStruct
    {
        public int VirtualKeyCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public int ExtraInfo;
    }
}