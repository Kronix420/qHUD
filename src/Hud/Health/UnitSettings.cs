﻿namespace qHUD.Hud.Health
{
    using Settings;
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

        public RangeNode<float> Width { get; set; }
        public RangeNode<float> Height { get; set; }
        public ColorNode Color { get; set; }
        public ColorNode Outline { get; set; }
        public RangeNode<int> TextSize { get; set; }
    }
}