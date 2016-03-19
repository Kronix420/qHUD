using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Loot
{
    public sealed class ItemAlertSettings : SettingsBase
    {
        public ItemAlertSettings()
        {
            Enable = true;
            ShowItemOnMap = true;
            ShowText = true;
            PlaySound = true;
            LootIcon = new RangeNode<int>(7, 1, 14);
            SoundVolume = new RangeNode<int>(20, 0, 100);
            TextSize = new RangeNode<int>(16, 10, 50);
            WithBorder = true;
            WithSound = false;
            Alternative = true;
            FilePath = "config/thisBest.filter";
        }

        public ToggleNode ShowItemOnMap { get; set; }
        public ToggleNode ShowText { get; set; }
        public ToggleNode PlaySound { get; set; }
        public RangeNode<int> LootIcon { get; set; }
        public RangeNode<int> SoundVolume { get; set; }
        public RangeNode<int> TextSize { get; set; }
        public ToggleNode WithBorder { get; set; }
        public ToggleNode WithSound { get; set; }
        public ToggleNode Alternative { get; set; }
        public FileNode FilePath { get; set; }
    }
}