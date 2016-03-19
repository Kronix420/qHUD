namespace PoeHUD.Poe.Elements
{
    public class ItemOnGroundTooltip : Element
    {
        //public Entity Item
        //{
        //    get
        //    {
        //        var address = M.ReadInt(Address + OffsetBuffers, 0, 0x8EC, 0x904);
        //        var entity = GetObject<Entity>(address);
        //        return entity;
        //    }
        //}
        public Element Tooltip => GetChildAtIndex(0);
        public Element TooltipUI => GetChildAtIndex(0).GetChildAtIndex(0);
    }
}