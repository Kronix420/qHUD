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
        private readonly Dictionary<string, PreloadConfigLine> alertStrings;
        private int lastCount, lastAddress;
        public static Color hasCorruptedArea { get; set; }

        public PreloadAlertPlugin(GameController gameController, Graphics graphics, PreloadAlertSettings settings)
            : base(gameController, graphics, settings)
        {
            alerts = new HashSet<PreloadConfigLine>();
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
            try
            {
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
            }
            catch
            {
                // do nothing
            }
        }

        private void OnAreaChange(AreaController area)
        {
            alerts.Clear();
            lastCount = 0;
            lastAddress = 0;
        }

        private void Parse()
        {
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
            if (alertStrings.ContainsKey(text)) { alerts.Add(alertStrings[text]);
                return;
            }
            if (text.Contains("human_heart") || text.Contains("Demonic_NoRain.ogg")) { hasCorruptedArea = Settings.HasCorruptedArea;
                return;
            }

            Dictionary<string, PreloadConfigLine> PerandusLeague = new Dictionary<string, PreloadConfigLine>
            {
                {"Metadata/NPC/League/Cadiro", new PreloadConfigLine { Text = "Cadiro Trader", FastColor = () => Settings.CadiroTrader }},

                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss1", new PreloadConfigLine { Text = "Platinia, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss2", new PreloadConfigLine { Text = "Auriot, Prospero's Furnace Guardian", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss3", new PreloadConfigLine { Text = "Rhodion, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss4", new PreloadConfigLine { Text = "Osmea, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss4Clone", new PreloadConfigLine { Text = "Osmea, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss5", new PreloadConfigLine { Text = "Pallias, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss6", new PreloadConfigLine { Text = "Argient, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardMapBoss7", new PreloadConfigLine { Text = "Rheniot, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant1", new PreloadConfigLine { Text = "Junith Perandus, Keeper of Vaults", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant2", new PreloadConfigLine { Text = "Tantalo Perandus, Seller of Secrets", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant3", new PreloadConfigLine { Text = "Actaeo Perandus, Master of Beasts", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant4", new PreloadConfigLine { Text = "Vitorica Perandus, Maker of Marvels", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant4Clone", new PreloadConfigLine { Text = "Vitorica Perandus, Maker of Marvels", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant5", new PreloadConfigLine { Text = "Stasius Perandus, Merchant of Corpses", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant6", new PreloadConfigLine { Text = "Darsia Perandus, Collector of Debts", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardLieutenant7", new PreloadConfigLine { Text = "Milo Perandus, Handler of Swords", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardBasic1", new PreloadConfigLine { Text = "Celona, Vault Sentry", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardBasic2", new PreloadConfigLine { Text = "Hortus, Knee Breaker", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardBasic3", new PreloadConfigLine { Text = "Kuto, Hired Muscle", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardBasic4", new PreloadConfigLine { Text = "Luthis, Bounty Hunter", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardBasic5", new PreloadConfigLine { Text = "Belatra, Hired Assassin", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSupport1", new PreloadConfigLine { Text = "Liana, Indebted Peasant", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSupport2", new PreloadConfigLine { Text = "Marius, Indebted Smuggler", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSupport3", new PreloadConfigLine { Text = "Vera, Indebted Aristocrat", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSupport4", new PreloadConfigLine { Text = "Percia, Indebted Poacher", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss1", new PreloadConfigLine { Text = "Sutrus, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss2", new PreloadConfigLine { Text = "Otairo, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss3", new PreloadConfigLine { Text = "Eiphirio, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss4", new PreloadConfigLine { Text = "Artensia, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss5_", new PreloadConfigLine { Text = "Anaveli, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss6", new PreloadConfigLine { Text = "Rothus, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss7", new PreloadConfigLine { Text = "Arinea, Vault Binder", FastColor = () => Settings.PerandusGuards }},
                {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss8", new PreloadConfigLine { Text = "Meritania, Vault Binder", FastColor = () => Settings.PerandusGuards }}
            };
            PreloadConfigLine perandus = PerandusLeague.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (perandus != null) { alerts.Add(perandus);
                return;
            }
            
            Dictionary<string, PreloadConfigLine> Strongboxes = new Dictionary<string, PreloadConfigLine>
            {
                {"Metadata/Chests/StrongBoxes/StrongboxDivination", new PreloadConfigLine { Text = "Diviner's Strongbox", FastColor = () => Settings.DivinersStrongbox }},
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
                {"Metadata/Chests/CopperChestEpic3", new PreloadConfigLine { Text = "Epic Chest", FastColor = () => Settings.EpicStrongbox }},
                {"Metadata/Chests/StrongBoxes/PerandusBox", new PreloadConfigLine { Text = "Perandus Strongbox", FastColor = () => Settings.PerandusStrongbox }},
                {"Metadata/Chests/StrongBoxes/KaomBox", new PreloadConfigLine { Text = "Kaom Strongbox", FastColor = () => Settings.KaomStrongbox }},
                {"Metadata/Chests/StrongBoxes/MalachaisBox", new PreloadConfigLine { Text = "Malachai Strongbox", FastColor = () => Settings.MalachaiStrongbox }},

                {"Metadata/Monsters/Spiders/SpiderBarrelOfSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox I", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/BarrelOfSpidersDaemonNormal2", new PreloadConfigLine { Text = "Unique Strongbox II", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/BossDaemonBarrelSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox III", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/ChestDaemonPoison", new PreloadConfigLine { Text = "Unique Strongbox IV", FastColor = () => Settings.MalachaiStrongbox }}
            };
            PreloadConfigLine strongboxes = Strongboxes.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (strongboxes != null) { alerts.Add(strongboxes);
                return;
            }

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
            PreloadConfigLine masters = Masters.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (masters != null) { alerts.Add(masters);
                return;
            }

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
                {"ExileWitch4", new PreloadConfigLine { Text = "Exile Dena Lorenni", FastColor = () => Settings.DenaLorenni }},
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
                {"ExileScion3", new PreloadConfigLine { Text = "Exile Lael Furia", FastColor = () => Settings.LaelFuria }},
                {"ExileScion4", new PreloadConfigLine { Text = "Exile Vanth Agiel", FastColor = () => Settings.VanthAgiel }}
            };
            PreloadConfigLine exiles = Exiles.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (exiles != null) { alerts.Add(exiles);
            }
        }
    }
}


//using PoeHUD.Controllers;
//using PoeHUD.Framework;
//using PoeHUD.Framework.Helpers;
//using PoeHUD.Hud.UI;
//using PoeHUD.Models;
//using SharpDX;
//using SharpDX.Direct3D9;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;

//namespace PoeHUD.Hud.Preload
//{
//    public class PreloadAlertPlugin : SizedPlugin<PreloadAlertSettings>
//    {
//        private readonly HashSet<PreloadConfigLine> alerts;
//        private readonly Dictionary<string, PreloadConfigLine> alertStrings;
//        private bool areaChanged = true;
//        private DateTime maxParseTime = DateTime.Now;
//        private int lastCount;
//        public static Color hasCorruptedArea { get; set; }

//        public PreloadAlertPlugin(GameController gameController, Graphics graphics, PreloadAlertSettings settings)
//            : base(gameController, graphics, settings)
//        {
//            alerts = new HashSet<PreloadConfigLine>();
//            alertStrings = LoadConfig("config/preload_alerts.txt");
//            GameController.Area.OnAreaChange += OnAreaChange;
//        }

//        public Dictionary<string, PreloadConfigLine> LoadConfig(string path)
//        {
//            return LoadConfigBase(path, 3).ToDictionary(line => line[0], line =>
//            {
//                var preloadAlerConfigLine = new PreloadConfigLine
//                {
//                    Text = line[1],
//                    Color = line.ConfigColorValueExtractor(2)
//                };
//                return preloadAlerConfigLine;
//            });
//        }

//        public override void Render()
//        {
//            base.Render();
//            if (!Settings.Enable || WinApi.IsKeyDown(Keys.F10)) { return; }
//            if (areaChanged || WinApi.IsKeyDown(Keys.F5))
//            {
//                Parse();
//                lastCount = GetNumberOfObjects();
//            }
//            else if (DateTime.Now <= maxParseTime)
//            {
//                int count = GetNumberOfObjects();
//                if (lastCount != count)
//                {
//                    areaChanged = true;
//                }
//            }

//            if (alerts.Count <= 0) return;
//            Vector2 startPosition = StartDrawPointFunc();
//            Vector2 position = startPosition;
//            int maxWidth = 0;
//            foreach (Size2 size in alerts
//                .Select(preloadConfigLine => Graphics
//                    .DrawText(preloadConfigLine.Text, Settings.TextSize, position, preloadConfigLine.FastColor?
//                        .Invoke() ?? preloadConfigLine.Color ?? Settings.DefaultTextColor, FontDrawFlags.Right)))
//            {
//                maxWidth = Math.Max(size.Width, maxWidth);
//                position.Y += size.Height;
//            }
//            if (maxWidth <= 0) return;
//            var bounds = new RectangleF(startPosition.X - maxWidth - 45, startPosition.Y - 5,
//                maxWidth + 50, position.Y - startPosition.Y + 10);
//            Graphics.DrawImage("preload-start.png", bounds, Settings.BackgroundColor);
//            Graphics.DrawImage("preload-end.png", bounds, Settings.BackgroundColor);
//            Size = bounds.Size;
//            Margin = new Vector2(0, 5);
//        }

//        private int GetNumberOfObjects()
//        {
//            Memory memory = GameController.Memory;
//            return memory.ReadInt(memory.AddressOfProcess + memory.offsets.FileRoot, 12);
//        }

//        private void OnAreaChange(AreaController area)
//        {
//            maxParseTime = area.CurrentArea.TimeEntered.AddSeconds(10);
//            areaChanged = true;
//        }

//        private void Parse()
//        {
//            areaChanged = false; alerts.Clear();
//            Memory memory = GameController.Memory;
//            hasCorruptedArea = Settings.AreaTextColor;
//            int pFileRoot = memory.ReadInt(memory.AddressOfProcess + memory.offsets.FileRoot);
//            int count = memory.ReadInt(pFileRoot + 12);
//            int listIterator = memory.ReadInt(pFileRoot + 0x10);
//            int areaChangeCount = GameController.Game.AreaChangeCount;
//            for (int i = 0; i < count; i++)
//            {
//                listIterator = memory.ReadInt(listIterator);
//                if (memory.ReadInt(listIterator + 8) == 0 || memory.ReadInt(listIterator + 12, 36) != areaChangeCount) continue;
//                string text = memory.ReadStringU(memory.ReadInt(listIterator + 8));
//                if (text.Contains('@')) { text = text.Split('@')[0]; }
//                if (alertStrings.ContainsKey(text)) { alerts.Add(alertStrings[text]); }
//                if (text.Contains("human_heart") || text.Contains("Demonic_NoRain.ogg")) { hasCorruptedArea = Settings.HasCorruptedArea; }

//                Dictionary<string, PreloadConfigLine> PerandusLeague = new Dictionary<string, PreloadConfigLine>
//                {
//                    {"Metadata/NPC/League/Cadiro", new PreloadConfigLine { Text = "Cadiro Trader", FastColor = () => Settings.CadiroTrader }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss1", new PreloadConfigLine { Text = "Platinia, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss2", new PreloadConfigLine { Text = "Auriot, Prospero's Furnace Guardian", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss3", new PreloadConfigLine { Text = "Rhodion, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss4", new PreloadConfigLine { Text = "Osmea, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss4Clone", new PreloadConfigLine { Text = "Osmea, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss5", new PreloadConfigLine { Text = "Pallias, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss6", new PreloadConfigLine { Text = "Argient, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardMapBoss7", new PreloadConfigLine { Text = "Rheniot, Servant of Prospero", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant1", new PreloadConfigLine { Text = "Junith Perandus, Keeper of Vaults", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant2", new PreloadConfigLine { Text = "Tantalo Perandus, Seller of Secrets", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant3", new PreloadConfigLine { Text = "Actaeo Perandus, Master of Beasts", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant4", new PreloadConfigLine { Text = "Vitorica Perandus, Maker of Marvels", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant4Clone", new PreloadConfigLine { Text = "Vitorica Perandus, Maker of Marvels", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant5", new PreloadConfigLine { Text = "Stasius Perandus, Merchant of Corpses", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant6", new PreloadConfigLine { Text = "Darsia Perandus, Collector of Debts", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardLieutenant7", new PreloadConfigLine { Text = "Milo Perandus, Handler of Swords", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardBasic1", new PreloadConfigLine { Text = "Celona, Vault Sentry", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardBasic2", new PreloadConfigLine { Text = "Hortus, Knee Breaker", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardBasic3", new PreloadConfigLine { Text = "Kuto, Hired Muscle", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardBasic4", new PreloadConfigLine { Text = "Luthis, Bounty Hunter", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardBasic5", new PreloadConfigLine { Text = "Belatra, Hired Assassin", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSupport1", new PreloadConfigLine { Text = "Liana, Indebted Peasant", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSupport2", new PreloadConfigLine { Text = "Marius, Indebted Smuggler", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSupport3", new PreloadConfigLine { Text = "Vera, Indebted Aristocrat", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSupport4", new PreloadConfigLine { Text = "Percia, Indebted Poacher", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss1", new PreloadConfigLine { Text = "Sutrus, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss2", new PreloadConfigLine { Text = "Otairo, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss3", new PreloadConfigLine { Text = "Eiphirio, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss4", new PreloadConfigLine { Text = "Artensia, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss5_", new PreloadConfigLine { Text = "Anaveli, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss6", new PreloadConfigLine { Text = "Rothus, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss7", new PreloadConfigLine { Text = "Arinea, Vault Binder", FastColor = () => Settings.PerandusGuards }},
//                    {"Metadata/Monsters/Perandus/PerandusGuardSecondaryBoss8", new PreloadConfigLine { Text = "Meritania, Vault Binder", FastColor = () => Settings.PerandusGuards }}
//                };

//                PreloadConfigLine perandus = PerandusLeague.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
//                if (perandus != null) { alerts.Add(perandus); }

//                Dictionary<string, PreloadConfigLine> Strongboxes = new Dictionary<string, PreloadConfigLine>
//                {
//                    {"Metadata/Chests/StrongBoxes/StrongboxDivination", new PreloadConfigLine { Text = "Diviner's Strongbox", FastColor = () => Settings.DivinersStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Arcanist", new PreloadConfigLine { Text = "Arcanist's Strongbox", FastColor = () => Settings.ArcanistStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Artisan", new PreloadConfigLine { Text = "Artisan's Strongbox", FastColor = () => Settings.ArtisanStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Cartographer", new PreloadConfigLine { Text = "Cartographer's Strongbox", FastColor = () => Settings.CartographerStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Gemcutter", new PreloadConfigLine { Text = "Gemcutter's Strongbox", FastColor = () => Settings.GemcutterStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Jeweller", new PreloadConfigLine { Text = "Jeweller's Strongbox", FastColor = () => Settings.JewellerStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Arsenal", new PreloadConfigLine { Text = "Blacksmith's Strongbox", FastColor = () => Settings.BlacksmithStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Armory", new PreloadConfigLine { Text = "Armourer's Strongbox", FastColor = () => Settings.ArmourerStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Ornate", new PreloadConfigLine { Text = "Ornate Strongbox", FastColor = () => Settings.OrnateStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Large", new PreloadConfigLine { Text = "Large Strongbox", FastColor = () => Settings.LargeStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/Strongbox", new PreloadConfigLine { Text = "Simple Strongbox", FastColor = () => Settings.SimpleStrongbox }},
//                    {"Metadata/Chests/CopperChestEpic3", new PreloadConfigLine { Text = "Epic Chest", FastColor = () => Settings.EpicStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/PerandusBox", new PreloadConfigLine { Text = "Perandus Strongbox", FastColor = () => Settings.PerandusStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/KaomBox", new PreloadConfigLine { Text = "Kaom Strongbox", FastColor = () => Settings.KaomStrongbox }},
//                    {"Metadata/Chests/StrongBoxes/MalachaisBox", new PreloadConfigLine { Text = "Malachai Strongbox", FastColor = () => Settings.MalachaiStrongbox }},

//                    {"Metadata/Monsters/Spiders/SpiderBarrelOfSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox I", FastColor = () => Settings.MalachaiStrongbox }},
//                    {"Metadata/Monsters/Daemon/BarrelOfSpidersDaemonNormal2", new PreloadConfigLine { Text = "Unique Strongbox II", FastColor = () => Settings.MalachaiStrongbox }},
//                    {"Metadata/Monsters/Daemon/BossDaemonBarrelSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox III", FastColor = () => Settings.MalachaiStrongbox }},
//                    {"Metadata/Monsters/Daemon/ChestDaemonPoison", new PreloadConfigLine { Text = "Unique Strongbox IV", FastColor = () => Settings.MalachaiStrongbox }}
//                };
//                PreloadConfigLine strongboxes = Strongboxes.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
//                if (strongboxes != null) { alerts.Add(strongboxes); }

//                Dictionary<string, PreloadConfigLine> Masters = new Dictionary<string, PreloadConfigLine>
//                {
//                    {"Wild/StrDexInt", new PreloadConfigLine { Text = "Zana, Master Cartographer", FastColor = () => Settings.MasterZana }},
//                    {"Wild/Int", new PreloadConfigLine { Text = "Catarina, Master of the Dead", FastColor = () => Settings.MasterCatarina }},
//                    {"Wild/Dex", new PreloadConfigLine { Text = "Tora, Master of the Hunt", FastColor = () => Settings.MasterTora }},
//                    {"Wild/DexInt", new PreloadConfigLine { Text = "Vorici, Master Assassin", FastColor = () => Settings.MasterVorici }},
//                    {"Wild/Str", new PreloadConfigLine { Text = "Haku, Armourmaster", FastColor = () => Settings.MasterHaku }},
//                    {"Wild/StrInt", new PreloadConfigLine { Text = "Elreon, Loremaster", FastColor = () => Settings.MasterElreon }},
//                    {"Wild/Fish", new PreloadConfigLine { Text = "Krillson, Master Fisherman", FastColor = () => Settings.MasterKrillson }},
//                    {"MasterStrDex1", new PreloadConfigLine { Text = "Vagan, Weaponmaster (2HSword)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex2", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Staff)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex3", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Bow)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex4", new PreloadConfigLine { Text = "Vagan, Weaponmaster (DaggerRapier)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex5", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Blunt)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex6", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Blades)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex7", new PreloadConfigLine { Text = "Vagan, Weaponmaster (SwordAxe)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex8", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Punching)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex9", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Flickerstrike)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex10", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Elementalist)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex11", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Cyclone)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex12", new PreloadConfigLine { Text = "Vagan, Weaponmaster (PhysSpells)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex13", new PreloadConfigLine { Text = "Vagan, Weaponmaster (Traps)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex14", new PreloadConfigLine { Text = "Vagan, Weaponmaster (RighteousFire)", FastColor = () => Settings.MasterVagan }},
//                    {"MasterStrDex15", new PreloadConfigLine { Text = "Vagan, Weaponmaster (CastOnHit)", FastColor = () => Settings.MasterVagan }}

//                };
//                PreloadConfigLine masters = Masters.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
//                if (masters != null) { alerts.Add(masters); }

//                Dictionary<string, PreloadConfigLine> Exiles = new Dictionary<string, PreloadConfigLine>
//                {
//                    {"ExileRanger1", new PreloadConfigLine { Text = "Exile Orra Greengate", FastColor = () => Settings.OrraGreengate }},
//                    {"ExileRanger2", new PreloadConfigLine { Text = "Exile Thena Moga", FastColor = () => Settings.ThenaMoga }},
//                    {"ExileRanger3", new PreloadConfigLine { Text = "Exile Antalie Napora", FastColor = () => Settings.AntalieNapora }},
//                    {"ExileDuelist1", new PreloadConfigLine { Text = "Exile Torr Olgosso", FastColor = () => Settings.TorrOlgosso }},
//                    {"ExileDuelist2", new PreloadConfigLine { Text = "Exile Armios Bell", FastColor = () => Settings.ArmiosBell }},
//                    {"ExileDuelist4", new PreloadConfigLine { Text = "Exile Zacharie Desmarais", FastColor = () => Settings.ZacharieDesmarais }},
//                    {"ExileWitch1", new PreloadConfigLine { Text = "Exile Minara Anenima", FastColor = () => Settings.MinaraAnenima }},
//                    {"ExileWitch2", new PreloadConfigLine { Text = "Exile Igna Phoenix", FastColor = () => Settings.IgnaPhoenix }},
//                    {"ExileWitch4", new PreloadConfigLine { Text = "Exile Dena Lorenni", FastColor = () => Settings.DenaLorenni }},
//                    {"ExileMarauder1", new PreloadConfigLine { Text = "Exile Jonah Unchained", FastColor = () => Settings.JonahUnchained }},
//                    {"ExileMarauder2", new PreloadConfigLine { Text = "Exile Damoi Tui", FastColor = () => Settings.DamoiTui }},
//                    {"ExileMarauder3", new PreloadConfigLine { Text = "Exile Xandro Blooddrinker", FastColor = () => Settings.XandroBlooddrinker }},
//                    {"ExileMarauder5", new PreloadConfigLine { Text = "Exile Vickas Giantbone", FastColor = () => Settings.VickasGiantbone }},
//                    {"ExileTemplar1", new PreloadConfigLine { Text = "Exile Eoin Greyfur", FastColor = () => Settings.EoinGreyfur }},
//                    {"ExileTemplar2", new PreloadConfigLine { Text = "Exile Tinevin Highdove", FastColor = () => Settings.TinevinHighdove }},
//                    {"ExileTemplar4", new PreloadConfigLine { Text = "Exile Magnus Stonethorn", FastColor = () => Settings.MagnusStonethorn}},
//                    {"ExileShadow1_", new PreloadConfigLine { Text = "Exile Ion Darkshroud", FastColor = () => Settings.IonDarkshroud}},
//                    {"ExileShadow2", new PreloadConfigLine { Text = "Exile Ash Lessard", FastColor = () => Settings.AshLessard}},
//                    {"ExileShadow4", new PreloadConfigLine { Text = "Exile Wilorin Demontamer", FastColor = () => Settings.WilorinDemontamer}},
//                    {"ExileScion2", new PreloadConfigLine { Text = "Exile Augustina Solaria", FastColor = () => Settings.AugustinaSolaria}},
//                    {"ExileScion3", new PreloadConfigLine { Text = "Exile Lael Furia", FastColor = () => Settings.LaelFuria }},
//                    {"ExileScion4", new PreloadConfigLine { Text = "Exile Vanth Agiel", FastColor = () => Settings.VanthAgiel }}
//                };
//                PreloadConfigLine exiles = Exiles.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
//                if (exiles != null) { alerts.Add(exiles); }
//            }
//        }
//    }
//}