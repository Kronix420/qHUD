namespace qHUD.Hud.Interfaces
{
    using System.Collections.Generic;
    public interface IPluginWithMapIcons
    {
        IEnumerable<MapIcon> GetIcons();
    }
}