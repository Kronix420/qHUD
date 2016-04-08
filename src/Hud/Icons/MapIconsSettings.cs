using qHUD.Hud.Settings;

namespace qHUD.Hud.Icons
{
    public sealed class MapIconsSettings : SettingsBase
    {
        public MapIconsSettings()
        {
            Enable = true;
            IconsOnMinimap = true;
            IconsOnLargeMap = true;
        }

        public ToggleNode IconsOnMinimap { get; set; }
        public ToggleNode IconsOnLargeMap { get; set; }
    }
}