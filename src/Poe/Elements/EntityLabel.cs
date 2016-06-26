namespace qHUD.Poe.Elements
{
    public class EntityLabel : Element
    {
        public string Text
        {
            get
            {
                int Length = M.ReadInt(Address + 0xB10);
                if (Length <= 0 || Length > 256)
                {
                    return string.Empty;
                }
                return Length >= 8 ? M.ReadStringU(M.ReadInt(Address + 0xB00), Length * 2) : M.ReadStringU(Address + 0xB00, Length * 2);
            }
        }
    }
}