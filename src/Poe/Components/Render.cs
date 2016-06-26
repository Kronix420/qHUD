namespace qHUD.Poe.Components
{
    public class Render : Component
    {
        public float Z => Address != 0 ? M.ReadFloat(Address + 0x8C) : 0f;
    }
}