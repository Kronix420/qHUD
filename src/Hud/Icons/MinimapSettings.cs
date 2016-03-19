﻿using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Icons
{
    public sealed class MinimapSettings : SettingsBase
    {
        public MinimapSettings()
        {
            Enable = true;
            IconsOnMinimap = true;
        }

        public ToggleNode IconsOnMinimap { get; set; }
    }
}