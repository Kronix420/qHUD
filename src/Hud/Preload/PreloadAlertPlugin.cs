namespace qHUD.Hud.Preload
{
    using Controllers;
    using Framework;
    using Framework.Helpers;
    using Models;
    using Settings;
    using SharpDX;
    using SharpDX.Direct3D9;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using UI;

    public class PreloadAlertPlugin : SizedPlugin<PreloadAlertSettings>
    {
        private readonly SettingsHub settingsHub;
        private readonly HashSet<PreloadConfigLine> alerts;
        private readonly Dictionary<string, PreloadConfigLine> alertStrings;
        public static bool corruptedArea, unknownChest, holdKey;

        public PreloadAlertPlugin(GameController gameController, Graphics graphics, PreloadAlertSettings settings,
            SettingsHub settingsHub)
            : base(gameController, graphics, settings)
        {
            this.settingsHub = settingsHub;
            alerts = new HashSet<PreloadConfigLine>();
            alertStrings = LoadConfig("config/preload_alerts.txt");
            GameController.Area.OnAreaChange += OnAreaChange;
        }

        public Dictionary<string, PreloadConfigLine> LoadConfig(string path)
        {
            return LoadConfigBase(path, 3).ToDictionary(line => line[0], line =>
            {
                var preloadConfigLine = new PreloadConfigLine
                {
                    Text = line[1],
                    Color = line.ConfigColorValueExtractor(2)
                };
                return preloadConfigLine;
            });
        }

        public override void Render()
        {
            if (WinApi.IsKeyDown(Keys.F5))
            {
                ResetArea();
                Parse();
            }
            if (!holdKey && WinApi.IsKeyDown(Keys.F10))
            {
                holdKey = true;
                Settings.Enable.Value = !Settings.Enable.Value;
                SettingsHub.Save(settingsHub);
            }
            else if (holdKey && !WinApi.IsKeyDown(Keys.F10))
            {
                holdKey = false;
            }
            if (!Settings.Enable || GameController.Area.CurrentArea.IsTown)
            {
                Size = new Size2F();
                return;
            }

            if (alerts.Count <= 0)
            {
                Size = new Size2F();
                return;
            }
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

        private void ResetArea()
        {
            alerts.Clear();
            unknownChest = false;
            corruptedArea = false;
        }

        private void OnAreaChange(AreaController area)
        {
            ResetArea();
            Parse();
        }

        private void Parse()
        {
            Memory memory = GameController.Memory;
            int pFileRoot = memory.BaseAddress + memory.offsets.FileRoot;
            int count = memory.ReadInt(pFileRoot + 0x8);
            int areaChangeCount = GameController.Game.AreaChangeCount;
            int listIterator = memory.ReadInt(pFileRoot + 0x4, 0x0);
            for (int i = 0; i < count; i++)
            {
                listIterator = memory.ReadInt(listIterator);
                if (listIterator == 0) { return; }
                if (memory.ReadInt(listIterator + 0x8) == 0 || memory.ReadInt(listIterator + 0xC, 0x34) != areaChangeCount) continue;
                string text = memory.ReadStringU(memory.ReadInt(listIterator + 8));
                if (text.Contains('@')) { text = text.Split('@')[0]; }
                CheckForPreload(text);
            }
        }
        public void CheckForPreload(string text)
        {
            if (text.StartsWith("Metadata/Terrain/Doodads/vaal_sidearea_effects/soulcoaster")) { corruptedArea = true; }
            if (alertStrings.ContainsKey(text)) { alerts.Add(alertStrings[text]); }

            Dictionary<string, PreloadConfigLine> Strongboxes = new Dictionary<string, PreloadConfigLine>
            {
                ["Metadata/Chests/StrongBoxes/StrongboxDivination"] = new PreloadConfigLine { Text = "Diviner's Strongbox", FastColor = () => Settings.DivinersStrongbox },
                ["Metadata/Chests/StrongBoxes/Arcanist"] = new PreloadConfigLine { Text = "Arcanist's Strongbox", FastColor = () => Settings.ArcanistStrongbox },
                ["Metadata/Chests/StrongBoxes/Artisan"] = new PreloadConfigLine { Text = "Artisan's Strongbox", FastColor = () => Settings.ArtisanStrongbox },
                ["Metadata/Chests/StrongBoxes/Cartographer"] = new PreloadConfigLine { Text = "Cartographer's Strongbox", FastColor = () => Settings.CartographerStrongbox },
                ["Metadata/Chests/StrongBoxes/Gemcutter"] = new PreloadConfigLine { Text = "Gemcutter's Strongbox", FastColor = () => Settings.GemcutterStrongbox },
                ["Metadata/Chests/StrongBoxes/Jeweller"] = new PreloadConfigLine { Text = "Jeweller's Strongbox", FastColor = () => Settings.JewellerStrongbox },
                ["Metadata/Chests/StrongBoxes/Arsenal"] = new PreloadConfigLine { Text = "Blacksmith's Strongbox", FastColor = () => Settings.BlacksmithStrongbox },
                ["Metadata/Chests/StrongBoxes/Armory"] = new PreloadConfigLine { Text = "Armourer's Strongbox", FastColor = () => Settings.ArmourerStrongbox },
                ["Metadata/Chests/StrongBoxes/Ornate"] = new PreloadConfigLine { Text = "Ornate Strongbox", FastColor = () => Settings.OrnateStrongbox },
                ["Metadata/Chests/StrongBoxes/Large"] = new PreloadConfigLine { Text = "Large Strongbox", FastColor = () => Settings.LargeStrongbox },
                ["Metadata/Chests/StrongBoxes/Strongbox"] = new PreloadConfigLine { Text = "Simple Strongbox", FastColor = () => Settings.SimpleStrongbox },
                ["Metadata/Chests/CopperChests/CopperChestEpic3"] = new PreloadConfigLine { Text = "Epic Chest", FastColor = () => Settings.EpicStrongbox },
                ["Metadata/Chests/StrongBoxes/PerandusBox"] = new PreloadConfigLine { Text = "Perandus Strongbox", FastColor = () => Settings.PerandusStrongbox },
                ["Metadata/Chests/StrongBoxes/KaomBox"] = new PreloadConfigLine { Text = "Kaom Strongbox", FastColor = () => Settings.KaomStrongbox },
                ["Metadata/Chests/StrongBoxes/MalachaisBox"] = new PreloadConfigLine { Text = "Malachai Strongbox", FastColor = () => Settings.MalachaiStrongbox },
                ["Metadata/Monsters/Spiders/SpiderBarrelOfSpidersBoss"] = new PreloadConfigLine { Text = "Unique Strongbox I", FastColor = () => Settings.MalachaiStrongbox },
                ["Metadata/Monsters/Daemon/BarrelOfSpidersDaemonNormal2"] = new PreloadConfigLine { Text = "Unique Strongbox II", FastColor = () => Settings.MalachaiStrongbox },
                ["Metadata/Monsters/Daemon/BossDaemonBarrelSpidersBoss"] = new PreloadConfigLine { Text = "Unique Strongbox III", FastColor = () => Settings.MalachaiStrongbox }
            };
            PreloadConfigLine strongboxes_alert = Strongboxes.Where(kv => text.Content(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (strongboxes_alert != null && alerts.Add(strongboxes_alert))
            {
                unknownChest = true;
                if (alerts.Contains(new PreloadConfigLine { Text = "Identifying strongboxes...", FastColor = () => Settings.SimpleStrongbox }))
                {
                    alerts.Remove(new PreloadConfigLine { Text = "Identifying strongboxes...", FastColor = () => Settings.SimpleStrongbox });
                }
                alerts.Add(strongboxes_alert);
            }
            if (text.Contains("StrongBoxes") && !unknownChest)
            {
                alerts.Add(new PreloadConfigLine { Text = "Identifying strongboxes...", FastColor = () => Settings.SimpleStrongbox });
            }

            Dictionary<string, PreloadConfigLine> Masters = new Dictionary<string, PreloadConfigLine>
            {
                ["Wild/StrDexInt"] = new PreloadConfigLine { Text = "Zana, Master Cartographer", FastColor = () => Settings.MasterZana },
                ["Wild/Int"] = new PreloadConfigLine { Text = "Catarina, Master of the Dead", FastColor = () => Settings.MasterCatarina },
                ["Wild/Dex"] = new PreloadConfigLine { Text = "Tora, Master of the Hunt", FastColor = () => Settings.MasterTora },
                ["Wild/DexInt"] = new PreloadConfigLine { Text = "Vorici, Master Assassin", FastColor = () => Settings.MasterVorici },
                ["Wild/Str"] = new PreloadConfigLine { Text = "Haku, Armourmaster", FastColor = () => Settings.MasterHaku },
                ["Wild/StrInt"] = new PreloadConfigLine { Text = "Elreon, Loremaster", FastColor = () => Settings.MasterElreon },
                ["Wild/Fish"] = new PreloadConfigLine { Text = "Krillson, Master Fisherman", FastColor = () => Settings.MasterKrillson },
                ["MasterStrDex1"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (2HSword)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex2"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Staff)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex3"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Bow)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex4"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (DaggerRapier)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex5"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Blunt)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex6"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Blades)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex7"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (SwordAxe)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex8"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Punching)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex9"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Flickerstrike)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex10"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Elementalist)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex11"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Cyclone)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex12"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (PhysSpells)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex13"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (Traps)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex14"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (RighteousFire)", FastColor = () => Settings.MasterVagan },
                ["MasterStrDex15"] = new PreloadConfigLine { Text = "Vagan, Weaponmaster (CastOnHit)", FastColor = () => Settings.MasterVagan }
            };
            PreloadConfigLine masters_alert = Masters.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (masters_alert != null) { alerts.Add(masters_alert); }

            Dictionary<string, PreloadConfigLine> Exiles = new Dictionary<string, PreloadConfigLine>
            {
                ["ExileRanger1"] = new PreloadConfigLine { Text = "Exile Orra Greengate", FastColor = () => Settings.OrraGreengate },
                ["ExileRanger2"] = new PreloadConfigLine { Text = "Exile Thena Moga", FastColor = () => Settings.ThenaMoga },
                ["ExileRanger3"] = new PreloadConfigLine { Text = "Exile Antalie Napora", FastColor = () => Settings.AntalieNapora },
                ["ExileDuelist1"] = new PreloadConfigLine { Text = "Exile Torr Olgosso", FastColor = () => Settings.TorrOlgosso },
                ["ExileDuelist2"] = new PreloadConfigLine { Text = "Exile Armios Bell", FastColor = () => Settings.ArmiosBell },
                ["ExileDuelist4"] = new PreloadConfigLine { Text = "Exile Zacharie Desmarais", FastColor = () => Settings.ZacharieDesmarais },
                ["ExileWitch1"] = new PreloadConfigLine { Text = "Exile Minara Anenima", FastColor = () => Settings.MinaraAnenima },
                ["ExileWitch2"] = new PreloadConfigLine { Text = "Exile Igna Phoenix", FastColor = () => Settings.IgnaPhoenix },
                ["ExileMarauder1"] = new PreloadConfigLine { Text = "Exile Jonah Unchained", FastColor = () => Settings.JonahUnchained },
                ["ExileMarauder2"] = new PreloadConfigLine { Text = "Exile Damoi Tui", FastColor = () => Settings.DamoiTui },
                ["ExileMarauder3"] = new PreloadConfigLine { Text = "Exile Xandro Blooddrinker", FastColor = () => Settings.XandroBlooddrinker },
                ["ExileMarauder5"] = new PreloadConfigLine { Text = "Exile Vickas Giantbone", FastColor = () => Settings.VickasGiantbone },
                ["ExileTemplar1"] = new PreloadConfigLine { Text = "Exile Eoin Greyfur", FastColor = () => Settings.EoinGreyfur },
                ["ExileTemplar2"] = new PreloadConfigLine { Text = "Exile Tinevin Highdove", FastColor = () => Settings.TinevinHighdove },
                ["ExileTemplar4"] = new PreloadConfigLine { Text = "Exile Magnus Stonethorn", FastColor = () => Settings.MagnusStonethorn },
                ["ExileShadow1_"] = new PreloadConfigLine { Text = "Exile Ion Darkshroud", FastColor = () => Settings.IonDarkshroud },
                ["ExileShadow2"] = new PreloadConfigLine { Text = "Exile Ash Lessard", FastColor = () => Settings.AshLessard },
                ["ExileShadow4"] = new PreloadConfigLine { Text = "Exile Wilorin Demontamer", FastColor = () => Settings.WilorinDemontamer },
                ["ExileScion2"] = new PreloadConfigLine { Text = "Exile Augustina Solaria", FastColor = () => Settings.AugustinaSolaria },
                ["ExileWitch4"] = new PreloadConfigLine { Text = "Exile Dena Lorenni", FastColor = () => Settings.DenaLorenni },
                ["ExileScion4"] = new PreloadConfigLine { Text = "Exile Vanth Agiel", FastColor = () => Settings.VanthAgiel },
                ["ExileScion3"] = new PreloadConfigLine { Text = "Exile Lael Furia", FastColor = () => Settings.LaelFuria }
            };
            PreloadConfigLine exiles_alert = Exiles.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (exiles_alert != null) { alerts.Add(exiles_alert); }

            Dictionary<string, PreloadConfigLine> Tormented = new Dictionary<string, PreloadConfigLine>
            {
                ["Metadata/Monsters/Spirit/TormentedArsonist"] = new PreloadConfigLine { Text = "Tormented Arsonist", FastColor = () => Settings.TormentedArsonist },
                ["Metadata/Monsters/Spirit/TormentedFreezer"] = new PreloadConfigLine { Text = "Tormented Aurora Cultist", FastColor = () => Settings.TormentedFreezer },
                ["Metadata/Monsters/Spirit/TormentedBlasphemer"] = new PreloadConfigLine { Text = "Tormented Blasphemer", FastColor = () => Settings.TormentedBlasphemer },
                ["Metadata/Monsters/Spirit/TormentedCannibal"] = new PreloadConfigLine { Text = "Tormented Cannibal", FastColor = () => Settings.TormentedCannibal },
                ["Metadata/Monsters/Spirit/TormentedCharlatan"] = new PreloadConfigLine { Text = "Tormented Charlatan", FastColor = () => Settings.TormentedCharlatan },
                ["Metadata/Monsters/Spirit/TormentedCutthroat"] = new PreloadConfigLine { Text = "Tormented Cutthroat", FastColor = () => Settings.TormentedCutthroat },
                ["Metadata/Monsters/Spirit/TormentedEmbezzler"] = new PreloadConfigLine { Text = "Tormented Embezzler", FastColor = () => Settings.TormentedEmbezzler },
                ["Metadata/Monsters/Spirit/TormentedCounterfeiter"] = new PreloadConfigLine { Text = "Tormented Forger", FastColor = () => Settings.TormentedCounterfeiter },
                ["Metadata/Monsters/Spirit/TormentedMartyr"] = new PreloadConfigLine { Text = "Tormented Martyr", FastColor = () => Settings.TormentedMartyr },
                ["Metadata/Monsters/Spirit/TormentedMutilator"] = new PreloadConfigLine { Text = "Tormented Mutilator", FastColor = () => Settings.TormentedMutilator },
                ["Metadata/Monsters/Spirit/TormentedNecromancer"] = new PreloadConfigLine { Text = "Tormented Necromancer", FastColor = () => Settings.TormentedNecromancer },
                ["Metadata/Monsters/Spirit/TormentedPoisoner"] = new PreloadConfigLine { Text = "Tormented Poisoner", FastColor = () => Settings.TormentedPoisoner },
                ["Metadata/Monsters/Spirit/TormentedRogue"] = new PreloadConfigLine { Text = "Tormented Rogue", FastColor = () => Settings.TormentedRogue },
                ["Metadata/Monsters/Spirit/TormentedLibrarian"] = new PreloadConfigLine { Text = "Tormented Seditionist", FastColor = () => Settings.TormentedLibrarian },
                ["Metadata/Monsters/Spirit/TormentedSmuggler"] = new PreloadConfigLine { Text = "Tormented Smuggler", FastColor = () => Settings.TormentedSmuggler },
                ["Metadata/Monsters/Spirit/TormentedSpy"] = new PreloadConfigLine { Text = "Tormented Spy", FastColor = () => Settings.TormentedSpy },
                ["Metadata/Monsters/Spirit/TormentedExperimenter"] = new PreloadConfigLine { Text = "Tormented Storm Cultist", FastColor = () => Settings.TormentedExperimenter },
                ["Metadata/Monsters/Spirit/TormentedThief"] = new PreloadConfigLine { Text = "Tormented Thief", FastColor = () => Settings.TormentedThief },
                ["Metadata/Monsters/Spirit/TormentedThug"] = new PreloadConfigLine { Text = "Tormented Thug", FastColor = () => Settings.TormentedThug },
                ["Metadata/Monsters/Spirit/TormentedCorrupter"] = new PreloadConfigLine { Text = "Tormented Vaal Cultist", FastColor = () => Settings.TormentedCorrupter },
                ["Metadata/Monsters/Spirit/TormentedWarlord"] = new PreloadConfigLine { Text = "Tormented Warlord", FastColor = () => Settings.TormentedWarlord },
                ["Metadata/Monsters/Spirit/TormentedFisherman"] = new PreloadConfigLine { Text = "Tormented Illegal Fisherman", FastColor = () => Settings.TormentedFisherman }
            };
            PreloadConfigLine tormented_alert = Tormented.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (tormented_alert != null) { alerts.Add(tormented_alert); }
        }
    }
}