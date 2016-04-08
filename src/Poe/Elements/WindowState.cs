namespace qHUD.Poe.Elements

{
    public class WindowState : Element
    {
        public new bool IsVisibleLocal => M.ReadInt(Address + 0x860) == 1;
    }
}