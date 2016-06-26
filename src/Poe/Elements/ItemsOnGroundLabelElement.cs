namespace qHUD.Poe.Elements
{
    using System.Collections.Generic;
    public class ItemsOnGroundLabelElement : Element
    {
        public Entity ItemOnGround => ReadObject<Entity>(Address + 0xC);
        public new IEnumerable<ItemsOnGroundLabelElement> Children
        {
            get
            {
                int address = M.ReadInt(Address + OffsetBuffers + 0x1D4);
                for (int nextAddress = M.ReadInt(address); nextAddress != address; nextAddress = M.ReadInt(nextAddress))
                {
                    yield return GetObject<ItemsOnGroundLabelElement>(nextAddress);
                }
            }
        }
    }
}