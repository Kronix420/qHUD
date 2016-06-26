namespace qHUD.Poe.RemoteMemoryObjects
{
    using System.Collections.Generic;
    using Elements;
    public class IngameUIElements : RemoteMemoryObject
    {
        public Element QuestTracker => ReadObjectAt<Element>(0x9E0);
        public Element InventoryPanel => ReadObjectAt<Element>(0xA24);
        public Element TreePanel => ReadObjectAt<Element>(0xA38);
        public Map Map => ReadObjectAt<Map>(0xA50);
        public IEnumerable<ItemsOnGroundLabelElement> ItemsOnGroundLabels
        {
            get
            {
                var itemsOnGroundLabelRoot = ReadObjectAt<ItemsOnGroundLabelElement>(0xA54);
                return itemsOnGroundLabelRoot.Children;
            }
        }
        public Element GemLvlUpPanel => ReadObjectAt<Element>(0xB20);
        public ItemOnGroundTooltip ItemOnGroundTooltip => ReadObjectAt<ItemOnGroundTooltip>(0xB30);
    }
}