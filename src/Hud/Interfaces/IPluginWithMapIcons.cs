using System.Collections.Generic;

namespace qHUD.Hud.Interfaces
{
    public interface IPluginWithMapIcons
    {
        IEnumerable<MapIcon> GetIcons();
    }
}