﻿using System;
using qHUD.Controllers;
using qHUD.Hud.Interfaces;
using qHUD.Hud.Settings;
using qHUD.Hud.UI;
using SharpDX;

namespace qHUD.Hud
{
    public abstract class SizedPluginWithMapIcons<TSettings> : PluginWithMapIcons<TSettings>, IPanelChild
        where TSettings : SettingsBase
    {
        protected SizedPluginWithMapIcons(GameController gameController, Graphics graphics, TSettings settings)
            : base(gameController, graphics, settings)
        { }

        public Size2F Size { get; protected set; }
        public Func<Vector2> StartDrawPointFunc { get; set; }
        public Vector2 Margin { get; private set; }

        public override void Render()
        {
            Size = new Size2F();
            Margin = new Vector2(0, 0);
        }
    }
}