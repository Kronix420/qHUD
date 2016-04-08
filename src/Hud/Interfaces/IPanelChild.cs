using System;
using SharpDX;

namespace qHUD.Hud.Interfaces
{
    public interface IPanelChild
    {
        Size2F Size { get; }
        Func<Vector2> StartDrawPointFunc { get; set; }
        Vector2 Margin { get; }
    }
}