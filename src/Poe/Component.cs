namespace qHUD.Poe
{
    public abstract class Component : RemoteMemoryObject
    {
        protected Entity Owner => ReadObject<Entity>(Address + 4);
    }
}