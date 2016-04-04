using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PoeHUD.Hud.Preload
{
    public class PreloadAlertPlugin : SizedPlugin<PreloadAlertSettings>
    {
        private readonly HashSet<PreloadConfigLine> alerts;
        private readonly Dictionary<string, PreloadConfigLine> perandusStrings, alertStrings;
        private int lastCount, lastAddress;
        private bool unknownChest;
        public static Color hasCorruptedArea { get; set; }

        public PreloadAlertPlugin(GameController gameController, Graphics graphics, PreloadAlertSettings settings)
            : base(gameController, graphics, settings)
        {
            alerts = new HashSet<PreloadConfigLine>();
            perandusStrings = LoadConfig("config/perandus_alerts.txt");
            alertStrings = LoadConfig("config/preload_alerts.txt");
            GameController.Area.OnAreaChange += OnAreaChange;
        }

        public Dictionary<string, PreloadConfigLine> LoadConfig(string path)
        {
            return LoadConfigBase(path, 3).ToDictionary(line => line[0], line =>
            {
                var preloadAlerConfigLine = new PreloadConfigLine
                {
                    Text = line[1],
                    Color = line.ConfigColorValueExtractor(2)
                };
                return preloadAlerConfigLine;
            });
        }

        public override void Render()
        {
            //try
            //{
                base.Render();
                if (!Settings.Enable || WinApi.IsKeyDown(Keys.F10)) { return; }
                Parse();

                if (alerts.Count <= 0) return;
                Vector2 startPosition = StartDrawPointFunc();
                Vector2 position = startPosition;
                int maxWidth = 0;
                foreach (Size2 size in alerts
                    .Select(preloadConfigLine => Graphics
                        .DrawText(preloadConfigLine.Text, Settings.TextSize, position, preloadConfigLine.FastColor?
                            .Invoke() ?? preloadConfigLine.Color ?? Settings.DefaultTextColor, FontDrawFlags.Right)))
                {
                    maxWidth = Math.Max(size.Width, maxWidth);
                    position.Y += size.Height;
                }
                if (maxWidth <= 0) return;
                var bounds = new RectangleF(startPosition.X - maxWidth - 45, startPosition.Y - 5,
                    maxWidth + 50, position.Y - startPosition.Y + 10);
                Graphics.DrawImage("preload-start.png", bounds, Settings.BackgroundColor);
                Graphics.DrawImage("preload-end.png", bounds, Settings.BackgroundColor);
                Size = bounds.Size;
                Margin = new Vector2(0, 5);
            //}
            //catch { // do nothing
            //}
        }

        private void OnAreaChange(AreaController area)
        {
            alerts.Clear();
            lastCount = 0;
            lastAddress = 0;
            unknownChest = false;
        }

        private void Parse()
        {
            if (WinApi.IsKeyDown(Keys.F5)) // do a full refresh if F5 is hit
            {
                alerts.Clear();
                lastAddress = 0;
                lastCount = 0;
                unknownChest = false;
            }
            Memory memory = GameController.Memory;
            hasCorruptedArea = Settings.AreaTextColor;
            int pFileRoot = memory.ReadInt(memory.AddressOfProcess + memory.offsets.FileRoot);
            int count = memory.ReadInt(pFileRoot + 0xC); // check how many files are loaded
            if (count > lastCount) // if the file count has changed, check the newly loaded files
            {
                int areaChangeCount = GameController.Game.AreaChangeCount;
                var listIterator = lastAddress == 0 ? memory.ReadInt(pFileRoot + 0x10) : lastAddress;
                for (int i = 0; i < count; i++)
                {
                    listIterator = memory.ReadInt(listIterator);
                    if (listIterator == 0)
                    {
                        // address is null, something has gone wrong, start over
                        alerts.Clear();
                        lastCount = 0;
                        lastAddress = 0;
                        return;
                    }
                    lastAddress = listIterator;
                    if (memory.ReadInt(listIterator + 0x8) == 0 || memory.ReadInt(listIterator + 0xC, 0x24) != areaChangeCount) continue;
                    string text = memory.ReadStringU(memory.ReadInt(listIterator + 8));
                    if (text.Contains('@')) { text = text.Split('@')[0]; }
                    CheckForPreload(text);
                }
            }
            lastCount = count;
        }

        private void CheckForPreload(string text)
        {
            if (text.Contains("Metadata/Chests/PerandusChests/PerandusChest.ao") || perandusStrings.ContainsKey(text))
            {
                alerts.Add(perandusStrings[text]); return;
            }

            if (alertStrings.ContainsKey(text)) { alerts.Add(alertStrings[text]); return; }

            if (text.Contains("human_heart") || text.Contains("Demonic_NoRain.ogg"))
            {
                if (Settings.CorruptedTitle) { hasCorruptedArea = Settings.HasCorruptedArea; }
                else { alerts.Add(new PreloadConfigLine { Text = "Corrupted Area", FastColor = () => Settings.HasCorruptedArea }); }
                return;
            }

            Dictionary<string, PreloadConfigLine> Strongboxes = new Dictionary<string, PreloadConfigLine>
            {
                {"Metadata/Chests/StrongBoxes/Arcanist", new PreloadConfigLine { Text = "Arcanist's Strongbox", FastColor = () => Settings.ArcanistStrongbox }},
                {"Metadata/Chests/StrongBoxes/Artisan", new PreloadConfigLine { Text = "Artisan's Strongbox", FastColor = () => Settings.ArtisanStrongbox }},
                {"Metadata/Chests/StrongBoxes/Cartographer", new PreloadConfigLine { Text = "Cartographer's Strongbox", FastColor = () => Settings.CartographerStrongbox }},
                {"Metadata/Chests/StrongBoxes/Gemcutter", new PreloadConfigLine { Text = "Gemcutter's Strongbox", FastColor = () => Settings.GemcutterStrongbox }},
                {"Metadata/Chests/StrongBoxes/Jeweller", new PreloadConfigLine { Text = "Jeweller's Strongbox", FastColor = () => Settings.JewellerStrongbox }},
                {"Metadata/Chests/StrongBoxes/Arsenal", new PreloadConfigLine { Text = "Blacksmith's Strongbox", FastColor = () => Settings.BlacksmithStrongbox }},
                {"Metadata/Chests/StrongBoxes/Armory", new PreloadConfigLine { Text = "Armourer's Strongbox", FastColor = () => Settings.ArmourerStrongbox }},
                {"Metadata/Chests/StrongBoxes/Ornate", new PreloadConfigLine { Text = "Ornate Strongbox", FastColor = () => Settings.OrnateStrongbox }},
                {"Metadata/Chests/StrongBoxes/Large", new PreloadConfigLine { Text = "Large Strongbox", FastColor = () => Settings.LargeStrongbox }},
                {"Metadata/Chests/StrongBoxes/Strongbox", new PreloadConfigLine { Text = "Simple Strongbox", FastColor = () => Settings.SimpleStrongbox }},
                {"Metadata/Chests/CopperChests/CopperChestEpic3", new PreloadConfigLine { Text = "Epic Chest", FastColor = () => Settings.EpicStrongbox }},
                {"Metadata/Chests/StrongBoxes/PerandusBox", new PreloadConfigLine { Text = "Perandus Strongbox", FastColor = () => Settings.PerandusStrongbox }},
                {"Metadata/Chests/StrongBoxes/KaomBox", new PreloadConfigLine { Text = "Kaom Strongbox", FastColor = () => Settings.KaomStrongbox }},
                {"Metadata/Chests/StrongBoxes/MalachaisBox", new PreloadConfigLine { Text = "Malachai Strongbox", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Spiders/SpiderBarrelOfSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox I", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/BarrelOfSpidersDaemonNormal2", new PreloadConfigLine { Text = "Unique Strongbox II", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/BossDaemonBarrelSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox III", FastColor = () => Settings.MalachaiStrongbox }}
            };
            PreloadConfigLine strongboxes_alert = Strongboxes.Where(kv => text
                .StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (strongboxes_alert != null && Settings.Strongboxes) { alerts.Add(strongboxes_alert); return; }

            Dictionary<string, PreloadConfigLine> Masters = new Dictionary<string, PreloadConfigLine>
            {
                {"Wild/StrDexInt", new PreloadConfigLine { Text = "Zana, Master Cartographer", FastColor = () => Settings.MasterZana }},
                {"Wild/Int", new PreloadConfigLine { Text = "Catarina, Master of the Dead", FastColor = () => Settings.MasterCatarina }},
                {"Wild/Dex", new PreloadConfigLine { Text = "Tora, Master of the Hunt", FastColor = () => Settings.MasterTora }},
                {"Wild/DexInt", new PreloadConfigLine { Text = "Vorici, Master Assassin", FastColor = () => Settings.MasterVorici }},
                {"Wild/Str", new PreloadConfigLine { Text = "Haku, Armourmaster", FastColor = () => Settings.MasterHaku }},
                {"Wild/StrInt", new PreloadConfigLine { Text = "Elreon, Loremaster", FastColor = () => Settings.MasterElreon }},
                {"Wild/Fish", new PreloadConfigLine { Text = "Krillson, Master Fisherman", FastColor = () => Settings.MasterKrillson }},
                {"MasterStrDex1", new PreloadConfigLine { Text = "Vagan, Weaponmaster (2HSword)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex2", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Staff)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex3", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Bow)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex4", new PreloadConfigLine { Text = "Vagan, Weaponmaster (DaggerRapier)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex5", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Blunt)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex6", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Blades)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex7", new PreloadConfigLine { Text = "Vagan, Weaponmaster (SwordAxe)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex8", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Punching)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex9", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Flickerstrike)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex10", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Elementalist)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex11", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Cyclone)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex12", new PreloadConfigLine { Text = "Vagan, Weaponmaster (PhysSpells)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex13", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Traps)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex14", new PreloadConfigLine { Text = "Vagan, Weaponmaster (RighteousFire)", FastColor = () => Settings.MasterVagan }},
                {"MasterStrDex15", new PreloadConfigLine { Text = "Vagan, Weaponmaster (CastOnHit)", FastColor = () => Settings.MasterVagan }}
            };
            PreloadConfigLine masters_alert = Masters.Where(kv => text
                .EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (masters_alert != null && Settings.Masters) { alerts.Add(masters_alert); return; }

            Dictionary<string, PreloadConfigLine> Exiles = new Dictionary<string, PreloadConfigLine>
            {
                {"ExileRanger1", new PreloadConfigLine { Text = "Exile Orra Greengate", FastColor = () => Settings.OrraGreengate }},
                {"ExileRanger2", new PreloadConfigLine { Text = "Exile Thena Moga", FastColor = () => Settings.ThenaMoga }},
                {"ExileRanger3", new PreloadConfigLine { Text = "Exile Antalie Napora", FastColor = () => Settings.AntalieNapora }},
                {"ExileDuelist1", new PreloadConfigLine { Text = "Exile Torr Olgosso", FastColor = () => Settings.TorrOlgosso }},
                {"ExileDuelist2", new PreloadConfigLine { Text = "Exile Armios Bell", FastColor = () => Settings.ArmiosBell }},
                {"ExileDuelist4", new PreloadConfigLine { Text = "Exile Zacharie Desmarais", FastColor = () => Settings.ZacharieDesmarais }},
                {"ExileWitch1", new PreloadConfigLine { Text = "Exile Minara Anenima", FastColor = () => Settings.MinaraAnenima }},
                {"ExileWitch2", new PreloadConfigLine { Text = "Exile Igna Phoenix", FastColor = () => Settings.IgnaPhoenix }},
                {"ExileMarauder1", new PreloadConfigLine { Text = "Exile Jonah Unchained", FastColor = () => Settings.JonahUnchained }},
                {"ExileMarauder2", new PreloadConfigLine { Text = "Exile Damoi Tui", FastColor = () => Settings.DamoiTui }},
                {"ExileMarauder3", new PreloadConfigLine { Text = "Exile Xandro Blooddrinker", FastColor = () => Settings.XandroBlooddrinker }},
                {"ExileMarauder5", new PreloadConfigLine { Text = "Exile Vickas Giantbone", FastColor = () => Settings.VickasGiantbone }},
                {"ExileTemplar1", new PreloadConfigLine { Text = "Exile Eoin Greyfur", FastColor = () => Settings.EoinGreyfur }},
                {"ExileTemplar2", new PreloadConfigLine { Text = "Exile Tinevin Highdove", FastColor = () => Settings.TinevinHighdove }},
                {"ExileTemplar4", new PreloadConfigLine { Text = "Exile Magnus Stonethorn", FastColor = () => Settings.MagnusStonethorn}},
                {"ExileShadow1_", new PreloadConfigLine { Text = "Exile Ion Darkshroud", FastColor = () => Settings.IonDarkshroud}},
                {"ExileShadow2", new PreloadConfigLine { Text = "Exile Ash Lessard", FastColor = () => Settings.AshLessard}},
                {"ExileShadow4", new PreloadConfigLine { Text = "Exile Wilorin Demontamer", FastColor = () => Settings.WilorinDemontamer}},
                {"ExileScion2", new PreloadConfigLine { Text = "Exile Augustina Solaria", FastColor = () => Settings.AugustinaSolaria}},
                {"ExileWitch4", new PreloadConfigLine { Text = "Exile Dena Lorenni", FastColor = () => Settings.DenaLorenni }},
                {"ExileScion4", new PreloadConfigLine { Text = "Exile Vanth Agiel", FastColor = () => Settings.VanthAgiel }},
                {"ExileScion3", new PreloadConfigLine { Text = "Exile Lael Furia", FastColor = () => Settings.LaelFuria }}
            };
            PreloadConfigLine exiles_alert = Exiles.Where(kv => text
                .EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (exiles_alert != null && Settings.Exiles) { alerts.Add(exiles_alert); }
        }
    }
}