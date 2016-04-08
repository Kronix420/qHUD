using System;
using qHUD.Controllers;
using qHUD.Hud.Interfaces;
using qHUD.Hud.Settings;
using qHUD.Hud.UI;
using SharpDX;

namespace qHUD.Hud
{
    public abstract class SizedPlugin<TSettings> : Plugin<TSettings>, IPanelChild where TSettings : SettingsBase
    {
        protected SizedPlugin(GameController gameController, Graphics graphics, TSettings settings)
            : base(gameController, graphics, settings)
        { }

        public Size2F Size { get; set; }
        public Func<Vector2> StartDrawPointFunc { get; set; }
        public Vector2 Margin { get; set; }

        public override void Render()
        {
            Size = new Size2F();
            Margin = new Vector2(0, 0);
        }
    }
}