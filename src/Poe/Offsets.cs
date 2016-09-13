namespace qHUD.Poe
{
    using Framework;
    using Models;

    public class Offsets
    {
        public int AreaChangeCount { get; private set; }
        public int Base { get; private set; }
        public string ExeName { get; private set; }
        public int FileRoot { get; private set; }
        public int IgsDelta { get; private set; }
        public int IgsOffset { get; private set; }
        public int GarenaTWDelta = 0;
        public int IgsOffsetDelta => IgsOffset + IgsDelta;

        public static Offsets Regular = new Offsets { IgsOffset = 0, IgsDelta = 0, ExeName = "PathOfExile" };
        public static Offsets Steam = new Offsets { IgsOffset = 0x1C, IgsDelta = 0x4, ExeName = "PathOfExileSteam" };
        
        private static readonly Pattern basePtrPattern = new Pattern(new byte[]
        {
            0x50, 0x64, 0x89, 0x25, 0x00, 0x00, 0x00, 0x00,
            0x83, 0xEC, 0x20, 0xC7, 0x45, 0xF0, 0x00, 0x00,
            0x00, 0x00, 0xA1
        }, "xxxxxxxxxxxxxxxxxxx");

        private static readonly Pattern fileRootPattern = new Pattern(new byte[]
        {
            0xB7, 0x00, 0x00, 0x00, 0x00, 0xB9, 0x00, 0x00,
            0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0xFF,
            0x15
        }, "x????x????x????xx");
        
        private static readonly Pattern areaChangePattern = new Pattern(new byte[]
            {
                0x8B, 0x88, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00,
                0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00,
                0xFF, 0x05
            }, "xx????x????x????xx");

        public void DoPatternScans(Memory m)
        {
            int[] array = m.FindPatterns(basePtrPattern, fileRootPattern, areaChangePattern);
            Base = m.ReadInt(m.BaseAddress + array[0] + 0x13) - m.BaseAddress;
            FileRoot = m.ReadInt(m.BaseAddress + array[1] + 0x6) - m.BaseAddress;
            AreaChangeCount = m.ReadInt(m.BaseAddress + array[2] + 0x12) - m.BaseAddress;
        }
    }
}