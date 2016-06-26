namespace qHUD.Hud.Interfaces
{
    using System;
    using SharpDX;
    public interface IPanelChild
    {
        Size2F Size { get; }
        Func<Vector2> StartDrawPointFunc { get; set; }
        Vector2 Margin { get; }
    }
}