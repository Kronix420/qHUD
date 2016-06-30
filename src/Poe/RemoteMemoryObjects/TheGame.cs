namespace qHUD.Poe.RemoteMemoryObjects
{
    using Framework;
    public class TheGame : RemoteMemoryObject
    {
        public TheGame(Memory m)
        {
            M = m;
            Address = m.ReadInt(m.BaseAddress + Offsets.Base, 4, 0xFC);
            Game = this;
        }
        public IngameState IngameState => ReadObject<IngameState>(Address + 0x11C);
        public int AreaChangeCount => M.ReadInt(M.BaseAddress + Offsets.AreaChangeCount);
    }
}