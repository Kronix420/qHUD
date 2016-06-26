namespace qHUD.Hud.Interfaces
{
    using System;
    public interface IPlugin : IDisposable
    {
        void Render();
    }
}