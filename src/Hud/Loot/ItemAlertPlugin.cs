using Antlr4.Runtime;
using PoeFilterParser;
using PoeFilterParser.Model;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Models.Interfaces;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.Elements;
using SharpDX;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PoeHUD.Hud.Loot
{
    public class ItemAlertPlugin : SizedPluginWithMapIcons<ItemAlertSettings>
    {
        private readonly HashSet<long> playedSoundsCache;
        private readonly Dictionary<EntityWrapper, AlertDrawStyle> currentAlerts;
        private readonly HashSet<string> currencyNames;
        private Dictionary<int, ItemsOnGroundLabelElement> currentLabels;
        private PoeFilterVisitor visitor;

        public ItemAlertPlugin(GameController gameController, Graphics graphics, ItemAlertSettings settings)
            : base(gameController, graphics, settings)
        {
            playedSoundsCache = new HashSet<long>();
            currentAlerts = new Dictionary<EntityWrapper, AlertDrawStyle>();
            currentLabels = new Dictionary<int, ItemsOnGroundLabelElement>();
            GameController.Area.OnAreaChange += OnAreaChange;
            PoeFilterInit(settings.FilePath);
            settings.FilePath.OnFileChanged += () => PoeFilterInit(settings.FilePath);
        }

        private void PoeFilterInit(string path)
        {
            using (var fileStream = new StreamReader(path))
            {
                var input = new AntlrInputStream(fileStream.ReadToEnd());
                var lexer = new PoeFilterLexer(input);
                var tokens = new CommonTokenStream(lexer);
                var parser = new PoeFilterParser.Model.PoeFilterParser(tokens);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(new ErrorListener());
                var tree = parser.main();
                visitor = new PoeFilterVisitor(tree, GameController, Settings);
            }
        }

        public override void Dispose()
        {
            GameController.Area.OnAreaChange -= OnAreaChange;
        }

        public override void Render()
        {
            base.Render();
            if (!Settings.Enable || WinApi.IsKeyDown(Keys.F10) || GameController.Area.CurrentArea.IsTown || GameController.Area.CurrentArea.IsHideout) { return; }

            if (Settings.Enable)
            {
                Vector2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
                Vector2 position = StartDrawPointFunc();
                const int BOTTOM_MARGIN = 2;
                bool shouldUpdate = false;

                foreach (KeyValuePair<EntityWrapper, AlertDrawStyle> kv in currentAlerts.Where(x => x.Key.IsValid))
                {
                    string text = GetItemName(kv);
                    if (text == null)
                    {
                        continue;
                    }

                    ItemsOnGroundLabelElement entityLabel;
                    if (!currentLabels.TryGetValue(kv.Key.Address, out entityLabel))
                    {
                        shouldUpdate = true;
                    }
                    else
                    {
                        if (Settings.ShowText)
                            position = DrawText(playerPos, position, BOTTOM_MARGIN, kv, text);
                    }
                }
                Size = new Size2F(0, position.Y);

                if (shouldUpdate)
                {
                    currentLabels = GameController.Game.IngameState.IngameUi.ItemsOnGroundLabels
                        .GroupBy(y => y.ItemOnGround.Address).ToDictionary(y => y.Key, y => y.First());
                }
            }
        }

        private Vector2 DrawText(Vector2 playerPos, Vector2 position, int BOTTOM_MARGIN,
            KeyValuePair<EntityWrapper, AlertDrawStyle> kv, string text)
        {
            var padding = new Vector2(5, 2);
            Vector2 delta = kv.Key.GetComponent<Positioned>().GridPos - playerPos;
            Vector2 itemSize = DrawItem(kv.Value, delta, position, padding, text);
            if (itemSize != new Vector2())
            {
                position.Y += itemSize.Y + BOTTOM_MARGIN;
            }
            return position;
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            if (!Settings.Enable || entity == null || currentAlerts.ContainsKey(entity) || !entity.HasComponent<WorldItem>()) return;
            IEntity item = entity.GetComponent<WorldItem>().ItemEntity;
            BaseItemType bit = GameController.Files.BaseItemTypes.Translate(item.Path);
            if (bit == null) return;
            if (string.IsNullOrEmpty(Settings.FilePath)) return;
            var result = visitor.Visit(item);
            if (result == null) return;
            AlertDrawStyle drawStyle = result;
            PrepareForDrawingAndPlaySound(entity, drawStyle);
        }

        private void PrepareForDrawingAndPlaySound(EntityWrapper entity, AlertDrawStyle drawStyle)
        {
            currentAlerts.Add(entity, drawStyle);
            CurrentIcons[entity] = new MapIcon(entity, new HudTexture("currency.png", drawStyle.TextColor), () => Settings.ShowItemOnMap, Settings.LootIcon);

            if (Settings.PlaySound && !playedSoundsCache.Contains(entity.LongId))
            {
                playedSoundsCache.Add(entity.LongId);
                Sounds.AlertSound.Play(Settings.SoundVolume);
            }
        }

        protected override void OnEntityRemoved(EntityWrapper entity)
        {
            base.OnEntityRemoved(entity);
            currentAlerts.Remove(entity);
            currentLabels.Remove(entity.Address);
        }

        private Vector2 DrawItem(AlertDrawStyle drawStyle, Vector2 delta, Vector2 position, Vector2 padding, string text)
        {
            padding.X -= drawStyle.BorderWidth;
            padding.Y -= drawStyle.BorderWidth;
            double phi;
            double distance = delta.GetPolarCoordinates(out phi);
            float compassOffset = Settings.TextSize + 8;
            Vector2 textPos = position.Translate(-padding.X - compassOffset, padding.Y);
            Size2 textSize = Graphics.DrawText(text, Settings.TextSize, textPos, drawStyle.TextColor, FontDrawFlags.Right);
            if (textSize == new Size2()) { return new Vector2(); }
            float fullHeight = textSize.Height + 2 * padding.Y + 2 * drawStyle.BorderWidth;
            float fullWidth = textSize.Width + 2 * padding.X + 2 * drawStyle.BorderWidth + compassOffset;
            var boxRect = new RectangleF(position.X - fullWidth, position.Y, fullWidth - compassOffset, fullHeight);
            Graphics.DrawBox(boxRect, drawStyle.BackgroundColor);

            RectangleF rectUV = GetDirectionsUV(phi, distance);
            var rectangleF = new RectangleF(position.X - padding.X - compassOffset + 6, position.Y + padding.Y,
                textSize.Height, textSize.Height);
            Graphics.DrawImage("directions.png", rectangleF, rectUV);

            if (drawStyle.BorderWidth > 0)
            {
                Graphics.DrawFrame(boxRect, drawStyle.BorderWidth, drawStyle.BorderColor);
            }
            return new Vector2(fullWidth, fullHeight);
        }

        private string GetItemName(KeyValuePair<EntityWrapper, AlertDrawStyle> kv)
        {
            string text;
            Entity itemEntity = kv.Key.GetComponent<WorldItem>().ItemEntity;
            EntityLabel labelForEntity = GameController.EntityListWrapper.GetLabelForEntity(itemEntity);
            if (labelForEntity == null)
            {
                if (!itemEntity.IsValid)
                {
                    return null;
                }
                text = kv.Value.Text;
            }
            else
            {
                text = labelForEntity.Text;
            }
            return text;
        }

        private void OnAreaChange(AreaController area)
        {
            playedSoundsCache.Clear();
            currentLabels.Clear();
            currentAlerts.Clear();
            CurrentIcons.Clear();
        }
    }
}