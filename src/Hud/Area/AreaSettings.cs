using PoeHUD.Hud.Settings;
using SharpDX;

namespace PoeHUD.Hud.Area
{
    public sealed class AreaSettings : SettingsBase
    {
        public AreaSettings()
        {
            Enable = true;
            ShowLatency = true;
            ShowInTown = true;
            TextSize = new RangeNode<int>(16, 10, 20);
            BackgroundColor = new ColorBGRA(0, 0, 0, 255);
            AreaTextColor = new ColorBGRA(140, 200, 255, 255);
            LatencyTextColor = new ColorBGRA(220, 190, 130, 255);
        }

        public ToggleNode ShowInTown { get; set; }
        public ToggleNode ShowLatency { get; set; }
        public RangeNode<int> TextSize { get; set; }
        public ColorNode BackgroundColor { get; set; }
        public ColorNode AreaTextColor { get; set; }
        public ColorNode LatencyTextColor { get; set; }
    }
}