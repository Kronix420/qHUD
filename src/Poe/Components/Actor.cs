namespace qHUD.Poe.Components
{
    public class Actor : Component
    {
        public int ActionId => Address != 0 ? M.ReadInt(Address + 0x7C) : 1;
        public bool isMoving => (ActionId & 128) > 0;
    }
}