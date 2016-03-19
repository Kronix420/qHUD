using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;
using PoeHUD.Models;
using PoeHUD.Poe.RemoteMemoryObjects;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = SharpDX.Color;
using Graphics = PoeHUD.Hud.UI.Graphics;
using RectangleF = SharpDX.RectangleF;

namespace PoeHUD.Hud.Health
{
    public class HealthBarPlugin : Plugin<HealthBarSettings>
    {
        private readonly Dictionary<CreatureType, List<HealthBar>> healthBars;

        public HealthBarPlugin(GameController gameController, Graphics graphics, HealthBarSettings settings)
            : base(gameController, graphics, settings)
        {
            CreatureType[] types = Enum.GetValues(typeof(CreatureType)).Cast<CreatureType>().ToArray();
            healthBars = new Dictionary<CreatureType, List<HealthBar>>(types.Length);
            foreach (CreatureType type in types)
            {
                healthBars.Add(type, new List<HealthBar>());
            }
        }

        public override void Render()
        {
            if (!Settings.Enable || WinApi.IsKeyDown(Keys.F10) || !GameController.InGame ||
                !Settings.ShowInTown && GameController.Area.CurrentArea.IsTown ||
                !Settings.ShowInTown && GameController.Area.CurrentArea.IsHideout)
            { return; }
            RectangleF windowRectangle = GameController.Window.GetWindowRectangle();
            var windowSize = new Size2F(windowRectangle.Width / 2560, windowRectangle.Height / 1600);
            Camera camera = GameController.Game.IngameState.Camera;
            Func<HealthBar, bool> showHealthBar = x => x.IsShow(Settings.ShowEnemies);
            Parallel.ForEach(healthBars, x => x.Value.RemoveAll(hp => !hp.Entity.IsValid));
            foreach (HealthBar healthBar in healthBars.SelectMany(x => x.Value).AsParallel().AsOrdered()
                .Where(hp => showHealthBar(hp) && hp.Entity.IsAlive))
            {
                Vector3 worldCoords = healthBar.Entity.Pos;
                float scaledWidth = healthBar.Settings.Width * windowSize.Width;
                float scaledHeight = healthBar.Settings.Height * windowSize.Height;
                Color color = healthBar.Settings.Color;
                float hpPercent = healthBar.Life.HPPercentage;
                float esPercent = healthBar.Life.ESPercentage;
                float hpWidth = hpPercent * scaledWidth;
                float esWidth = esPercent * scaledWidth;
                if (healthBar.Type == CreatureType.Player)
                {
                    Vector2 screenCoords =
                        camera.WorldToScreen(
                            worldCoords.Translate(Settings.xPlayers, Settings.yPlayers, 0),
                            healthBar.Entity);
                    var bg = new RectangleF(screenCoords.X - scaledWidth + 60, screenCoords.Y - scaledHeight - 40, scaledWidth, scaledHeight);
                    DrawBackground(color, healthBar.Settings.Outline, bg, hpWidth, esWidth);
                }

                if (healthBar.Type != CreatureType.Player)
                {
                    Vector2 screenCoords =
                        camera.WorldToScreen(
                            worldCoords.Translate(Settings.xEnemies, Settings.yEnemies, - 150),
                            healthBar.Entity);
                    var bg = new RectangleF(screenCoords.X - scaledWidth / 2, screenCoords.Y - scaledHeight / 2, scaledWidth, scaledHeight);
                    DrawBackground(color, healthBar.Settings.Outline, bg, hpWidth, esWidth);
                }
            }
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            var healthbarSettings = new HealthBar(entity, Settings);
            if (healthbarSettings.IsValid)
            {
                healthBars[healthbarSettings.Type].Add(healthbarSettings);
            }
        }

        private void DrawBackground(Color color, Color outline, RectangleF bg, float hpWidth, float esWidth)
        {
            if (outline != Color.Black)
            {
                Graphics.DrawFrame(bg, 2, outline);
            }
            string healthBar = Settings.ShowIncrements ? "healthbar_increment.png" : "healthbar.png";
            Graphics.DrawImage("healthbar_bg.png", bg, color);
            var hpRectangle = new RectangleF(bg.X, bg.Y, hpWidth, bg.Height);
            Graphics.DrawImage(healthBar, hpRectangle, color, hpWidth * 10 / bg.Width);
            if (Settings.ShowES)
            {
                bg.Width = esWidth;
                Graphics.DrawImage("esbar.png", bg);
            }
        }
    }
}