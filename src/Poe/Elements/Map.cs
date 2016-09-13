namespace qHUD.Poe.Elements
{
    public class Map : Element
    {
        public float ShiftX => M.ReadFloat(M.ReadInt(Address + 0x204 + OffsetBuffers) + OffsetBuffers + 0x18C);
        public float ShiftY => M.ReadFloat(M.ReadInt(Address + 0x204 + OffsetBuffers) + OffsetBuffers + 0x190);
        public Element LargeMap => ReadObjectAt<Element>(0x204 + OffsetBuffers);
        public Element Minimap => ReadObjectAt<Element>(0x208 + OffsetBuffers);
        public Element OrangeWords => ReadObjectAt<Element>(0x214 + OffsetBuffers);
        public Element BlueWords => ReadObjectAt<Element>(0x234 + OffsetBuffers);
    }
}