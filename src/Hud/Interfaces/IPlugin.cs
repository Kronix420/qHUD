using System;

namespace qHUD.Hud.Interfaces
{
    public interface IPlugin : IDisposable
    {
        void Render();
    }
}