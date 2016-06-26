namespace qHUD.Poe.Components
{
    public class Life : Component
    {
        public int MaxHP => Address != 0 ? M.ReadInt(Address + 0x2C) : 1;
        public int CurHP => Address != 0 ? M.ReadInt(Address + 0x30) : 1;
        public int ReservedHP => Address != 0 ? M.ReadInt(Address + 0x38) : 0;
        public int MaxES => Address != 0 ? M.ReadInt(Address + 0x74) : 0;
        public int CurES => Address != 0 ? M.ReadInt(Address + 0x78) : 0;
        public float HPPercentage => CurHP / (float)(MaxHP - ReservedHP);

        public float ESPercentage
        {
            get
            {
                if (MaxES != 0)
                {
                    return CurES / (float)MaxES;
                }
                return 0f;
            }
        }
    }
}