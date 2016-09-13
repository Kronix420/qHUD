namespace qHUD.Hud.Trackers
{
    using Settings;
    using SharpDX;
    public sealed class MonsterTrackerSettings : SettingsBase
    {
        public MonsterTrackerSettings()
        {
            Enable = true;
            Monsters = true;
            Minions = true;
            PlaySound = true;
            ShowText = true;
            //RemainingShowText = true;
            SoundVolume = new RangeNode<int>(50, 0, 100);
            TextSize = new RangeNode<int>(24, 10, 50);
            //RemainingTextSize = new RangeNode<int>(20, 10, 50);
            BackgroundColor = new ColorBGRA(192, 192, 192, 230);
            //RemainingBackColor = new ColorBGRA(192, 192, 192, 230);
            TextPositionX = new RangeNode<int>(50, 0, 100);
            TextPositionY = new RangeNode<int>(85, 0, 100);
            //RemainingTextPosX = new RangeNode<int>(50, 0, 100);
            //RemainingTextPosY = new RangeNode<int>(90, 0, 100);
            DefaultTextColor = Color.Red;
            //RemainingTextColor = Color.Red;
            MinionsIcon = new RangeNode<int>(3, 1, 6);
            WhiteMobIcon = new RangeNode<int>(3, 1, 6);
            MagicMobIcon = new RangeNode<int>(5, 1, 10);
            RareMobIcon = new RangeNode<int>(7, 1, 14);
            UniqueMobIcon = new RangeNode<int>(9, 1, 18);
        }

        public ToggleNode Monsters { get; set; }
        public ToggleNode Minions { get; set; }
        public ToggleNode PlaySound { get; set; }
        public RangeNode<int> SoundVolume { get; set; }
        public ToggleNode ShowText { get; set; }
        //public ToggleNode RemainingShowText { get; set; }
        public RangeNode<int> TextSize { get; set; }
        //public RangeNode<int> RemainingTextSize { get; set; }
        public ColorNode DefaultTextColor { get; set; }
        //public ColorNode RemainingTextColor { get; set; }
        public ColorNode BackgroundColor { get; set; }
        //public ColorNode RemainingBackColor { get; set; }
        public RangeNode<int> TextPositionX { get; set; }
        public RangeNode<int> TextPositionY { get; set; }
        //public RangeNode<int> RemainingTextPosX { get; set; }
        //public RangeNode<int> RemainingTextPosY { get; set; }
        public RangeNode<int> MinionsIcon { get; set; }
        public RangeNode<int> WhiteMobIcon { get; set; }
        public RangeNode<int> MagicMobIcon { get; set; }
        public RangeNode<int> RareMobIcon { get; set; }
        public RangeNode<int> UniqueMobIcon { get; set; }
    }
}