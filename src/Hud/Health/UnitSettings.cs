using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Health
{
    public class UnitSettings : SettingsBase
    {
        public UnitSettings(uint color, uint outline)
        {
            Enable = true;
            Width = new RangeNode<float>(80, 50, 200);
            Height = new RangeNode<float>(20, 10, 50);
            Color = color;
            Outline = outline;
            TextSize = new RangeNode<int>(15, 10, 50);
        }

        public UnitSettings(uint color, uint outline, uint percentTextColor, bool showText)
          : this(color, outline){}

        public RangeNode<float> Width { get; set; }
        public RangeNode<float> Height { get; set; }
        public ColorNode Color { get; set; }
        public ColorNode Outline { get; set; }
        public RangeNode<int> TextSize { get; set; }
    }
}