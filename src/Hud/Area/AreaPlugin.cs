using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.Preload;
using PoeHUD.Hud.UI;
using PoeHUD.Poe.Components;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PoeHUD.Hud.Area
{
    public class AreaPlugin : SizedPlugin<AreaSettings>
    {
        private double levelXpPenalty, partyXpPenalty;

        public AreaPlugin(GameController gameController, Graphics graphics, AreaSettings settings)
            : base(gameController, graphics, settings)
        {
            GameController.Area.OnAreaChange += area => AreaChange();
        }

        public override void Render()
        {
            base.Render();
            if (!Settings.Enable || WinApi.IsKeyDown(Keys.F10))
            { return; }

            bool showInTown =
                !Settings.ShowInTown && GameController.Area.CurrentArea.IsTown ||
                    !Settings.ShowInTown && GameController.Area.CurrentArea.IsHideout;
            partyXpPenalty = PartyXpPenalty();
            Vector2 position = StartDrawPointFunc();
            string latency = $"({GameController.Game.IngameState.CurLatency})";
            string areaName = $"{GameController.Area.CurrentArea.DisplayName}";
            var xpReceiving = levelXpPenalty * partyXpPenalty;
            var titleArea = $"{areaName}  *{xpReceiving:p0}";
            var areaNameSize = Graphics.MeasureText(titleArea, Settings.TextSize);
            Color hasCorruptedArea = PreloadAlertPlugin.hasCorruptedArea;
            if (!showInTown)
            {
                float boxHeight = areaNameSize.Height;
                float boxWidth = MathHepler.Max(areaNameSize.Width);
                var bounds = new RectangleF(position.X - 134 - boxWidth, position.Y - 5, boxWidth + 140, boxHeight + 12);

                Graphics.DrawText(titleArea, Settings.TextSize, new Vector2(bounds.X + 134, position.Y), hasCorruptedArea);
                Graphics.DrawImage("preload-start.png", bounds, Settings.BackgroundColor);
                Graphics.DrawImage("preload-end.png", bounds, Settings.BackgroundColor);
                if (Settings.ShowLatency)
                {
                    Graphics.DrawText(latency, Settings.TextSize, new Vector2(bounds.X + 80, position.Y), Settings.LatencyTextColor);
                }
                Size = bounds.Size;
                Margin = new Vector2(0, 5);
            }
        }

        private double LevelXpPenalty()
        {
            int arenaLevel = GameController.Area.CurrentArea.RealLevel;
            int characterLevel = GameController.Player.GetComponent<Player>().Level;
            double safeZone = Math.Floor(Convert.ToDouble(characterLevel) / 16) + 3;
            double effectiveDifference = Math.Max(Math.Abs(characterLevel - arenaLevel) - safeZone, 0);
            double xpMultiplier = Math.Max(Math.Pow((characterLevel + 5) / (characterLevel + 5 + Math.Pow(effectiveDifference, 2.5)), 1.5), 0.01);
            return xpMultiplier;
        }

        private double PartyXpPenalty()
        {
            var levels = GameController.Entities.Where(y => y.HasComponent<Player>()).Select(y => y.GetComponent<Player>().Level).ToList();
            int characterLevel = GameController.Player.GetComponent<Player>().Level;
            double partyXpPenalty = Math.Pow(characterLevel + 10, 2.71) / levels.Sum(level => Math.Pow(level + 10, 2.71));
            return partyXpPenalty * levels.Count;
        }

        private void AreaChange()
        {
            if (GameController.InGame)
            {
                levelXpPenalty = LevelXpPenalty();
            }
        }
    }
}