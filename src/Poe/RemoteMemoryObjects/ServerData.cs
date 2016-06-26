namespace qHUD.Poe.RemoteMemoryObjects
{
    public class ServerData : RemoteMemoryObject
    {
        public bool IsInGame => M.ReadInt(Address + 0x36B4) == 3;
    }
}