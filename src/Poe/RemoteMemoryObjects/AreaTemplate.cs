namespace qHUD.Poe.RemoteMemoryObjects
{
    public class AreaTemplate : RemoteMemoryObject
    {
        public string Name => M.ReadStringU(M.ReadInt(Address + 4));
        public int Act => M.ReadInt(Address + 8);
        public bool IsTown => M.ReadByte(Address + 0xC) == 1;
        public bool HasWaypoint => M.ReadByte(Address + 0xD) == 1;
        public int NominalLevel => M.ReadInt(Address + 0x16);
    }
}