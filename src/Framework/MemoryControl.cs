namespace qHUD.Framework
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    public class MemoryControl
    {
        private static MemoryControl memoryControl;
        private long lastTime;

        private MemoryControl()
        {
            lastTime = DateTime.Now.Ticks;
            Application.Idle += delegate
            {
                long ticks = DateTime.Now.Ticks;
                if (ticks - lastTime <= 10000000L) return;
                lastTime = ticks;
                MemoryFree();
            };
        }

        private static void MemoryFree()
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                WinApi.SetProcessWorkingSetSize(currentProcess.Handle, -1, -1);
            }
        }

        public static void Start()
        {
            if (memoryControl == null && Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                memoryControl = new MemoryControl();
            }
        }
    }
}