namespace qHUD.Poe.RemoteMemoryObjects
{
    using Models.Enums;
    public class IngameState : RemoteMemoryObject
    {
        public Camera Camera => GetObject<Camera>(Address + 0x1584 + Offsets.IgsOffsetDelta);
        public IngameData Data => ReadObject<IngameData>(Address + 0x138 + Offsets.IgsOffset);
        public bool InGame => ServerData.IsInGame;
        public ServerData ServerData => ReadObjectAt<ServerData>(0x13C + Offsets.IgsOffset);
        public IngameUIElements IngameUi => ReadObjectAt<IngameUIElements>(0x5E8 + Offsets.IgsOffset);
        public Element UIHover => ReadObjectAt<Element>(0xC14 + Offsets.IgsOffset);
        public int EntityLabelMap => M.ReadInt(Address + 0x50, 0x964);
        public DiagnosticInfoType DiagnosticInfoType => (DiagnosticInfoType)M.ReadInt(Address + 0xC80 + Offsets.IgsOffset);
        public DiagnosticElement LatencyRectangle => GetObjectAt<DiagnosticElement>(0xE98 + Offsets.IgsOffset);
        public DiagnosticElement FPSRectangle => GetObjectAt<DiagnosticElement>(0x1510 + Offsets.IgsOffset);
        public float CurLatency => M.ReadFloat(Address + 0xC90 + 0x13C + Offsets.IgsOffset);
        public string RemainingMonsters => M.ReadStringU(0x5E3528C0);
    }
}