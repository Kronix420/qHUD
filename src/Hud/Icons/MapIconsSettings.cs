namespace qHUD.Hud.Icons
{
    using Settings;
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