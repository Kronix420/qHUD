using qHUD.Framework;
using qHUD.Models;

namespace qHUD.Poe
{
    public class Offsets
    {
        public static Offsets Regular = new Offsets { IgsOffset = 0, IgsDelta = 0, ExeName = "PathOfExile"};
        public static Offsets Steam = new Offsets { IgsOffset = 0x1C, IgsDelta = 0x4, ExeName = "PathOfExileSteam"};

        private static readonly Pattern basePtrPattern = new Pattern
            (@"\x50\x64\x89\x25\x00\x00\x00\x00\x81\xec\xb0\x00\x00\x00\xa1\x00\x00\x00\x00\x85\xc0\x0f\x95\xc1\x84\xc9\x56\x0f\x94\xc1\x84\xc9",
                "xxxxxxxxxxxxxxx????xxxxxxxxxxxxx");

        private static readonly Pattern fileRootPattern = new Pattern
            (@"\xc6\x45\xfc\x00\xe8\x00\x00\x00\x00\x83\xc4\x00\x68\x00\x00\x00\x00\x51\x8d\x4d\x00\xe8",
                "xxxxx????xx?x????xxx?x");

        private static readonly Pattern areaChangePattern = new Pattern
            (@"\x8b\x88\x00\x00\x00\x00\xe8\x00\x00\x00\x00\xff\x05\x00\x00\x00\x00\xbe\x00\x00\x00\x00\x8b\x0e\x85\xc9",
                "xx????x????xx????x????xxxx");

        public int AreaChangeCount { get; private set; }
        public int Base { get; private set; }
        public string ExeName { get; private set; }
        public int FileRoot { get; private set; }
        public int IgsDelta { get; private set; }
        public int IgsOffset { get; private set; }
        public int IgsOffsetDelta => IgsOffset + IgsDelta;

        public void DoPatternScans(Memory m)
        {
            int[] array = m.FindPatterns(basePtrPattern, fileRootPattern, areaChangePattern);
            Base = m.ReadInt(m.AddressOfProcess + array[0] + 0x0F) - m.AddressOfProcess;
            FileRoot = array[1] + 0x0D;
            AreaChangeCount = m.ReadInt(m.AddressOfProcess + array[2] + 0x0D) - m.AddressOfProcess;
        }
    }
}