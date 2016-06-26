namespace qHUD.Poe.Components
{
    public class Player : Component
    {
        public int Level => Address != 0 ? M.ReadInt(Address + 0x40) : 1;
    }
}