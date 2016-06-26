namespace qHUD.Hud.Trackers
{
    using Settings;
    using SharpDX;
    public sealed class PoiTrackerSettings : SettingsBase
    {
        public PoiTrackerSettings()
        {
            Enable = true;
            Masters = true;
            Chests = true;
            Strongboxes = true;
            MastersIcon = new RangeNode<int>(8, 1, 16);
            CadiroIcon = new RangeNode<int>(8, 1, 16);
            StrongboxesIcon = new RangeNode<int>(14, 1, 28);
            ChestsIcon = new RangeNode<int>(3, 1, 6);


        }
        public ToggleNode Masters { get; set; }
        public ToggleNode Chests { get; set; }
        public ToggleNode Strongboxes { get; set; }
        public RangeNode<int> MastersIcon { get; set; }
        public RangeNode<int> CadiroIcon { get; set; }
        public RangeNode<int> ChestsIcon { get; set; }
        public RangeNode<int> StrongboxesIcon { get; set; }
    }
}