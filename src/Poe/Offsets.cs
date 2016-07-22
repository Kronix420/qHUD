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
        public int IgsOffsetDelta => IgsOffset + IgsDelta;

        public static Offsets Regular = new Offsets { IgsOffset = 0, IgsDelta = 0, ExeName = "PathOfExile", AreaChangeCount = 0xA73168 };
        public static Offsets Steam = new Offsets { IgsOffset = 0x1C, IgsDelta = 0x4, ExeName = "PathOfExileSteam", AreaChangeCount = 0xA7E168 };

        private static readonly Pattern basePtrPattern = new Pattern
            (@"\x50\x64\x89\x00\x00\x00\x00\x00\x81\xec\x00\x00\x00\x00\xa1\x00\x00\x00\x00\x85\xc0\x0f\x95\x00\x84\xc9",
                "xxx?????xx????x????xxxx?xx");

        private static readonly Pattern fileRootPattern = new Pattern
            (@"\xb7\x00\x00\x00\x00\xb9\x00\x00\x00\x00\xe8\x00\x00\x00\x00\xff\x15",
                "x????x????x????xx");

        public void DoPatternScans(Memory m)
        {
            int[] array = m.FindPatterns(basePtrPattern, fileRootPattern);
            Base = m.ReadInt(m.BaseAddress + array[0] + 0x0F) - m.BaseAddress;
            FileRoot = m.ReadInt(m.BaseAddress + array[1] + 0x6) - m.BaseAddress;
        }
    }
}