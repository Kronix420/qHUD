namespace qHUD.Poe.Elements
{
    public class Inventory : Element
    {
        public RemoteMemoryObjects.Inventory InventoryModel => ReadObject<RemoteMemoryObjects.Inventory>(Address + 0x984);
    }
}