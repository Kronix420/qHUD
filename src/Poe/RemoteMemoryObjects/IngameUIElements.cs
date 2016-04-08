using System.Collections.Generic;
using qHUD.Poe.Elements;

namespace qHUD.Poe.RemoteMemoryObjects
{
    public class IngameUIElements : RemoteMemoryObject
    {
        public Element QuestTracker => ReadObjectAt<Element>(0xF4);
        public Element InventoryPanel => ReadObjectAt<Element>(0x100);
        public Element TreePanel => ReadObjectAt<Element>(0x114);
        public Map Map => ReadObjectAt<Map>(0x12C);
        public IEnumerable<ItemsOnGroundLabelElement> ItemsOnGroundLabels
        {
            get
            {
                var itemsOnGroundLabelRoot = ReadObjectAt<ItemsOnGroundLabelElement>(0x130);
                return itemsOnGroundLabelRoot.Children;
            }
        }
        public Element OpenLeftPanel => ReadObjectAt<Element>(0x160);
        public Element OpenRightPanel => ReadObjectAt<Element>(0x164);
        public Element GemLvlUpPanel => ReadObjectAt<Element>(0x224);
        public ItemOnGroundTooltip ItemOnGroundTooltip => ReadObjectAt<ItemOnGroundTooltip>(0x234);
    }
}