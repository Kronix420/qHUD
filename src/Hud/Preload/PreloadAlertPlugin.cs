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
        private bool unknownChest;
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
                    Text = line[1], Color = line.ConfigColorValueExtractor(2)
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
                if (!GameController.Area.CurrentArea.IsTown || !GameController.Area.CurrentArea.IsHideout) { Parse(); }

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
            alerts.Clear(); lastCount = 0; lastAddress = 0; unknownChest = false;
        }

        private void Parse()
        {
            if (WinApi.IsKeyDown(Keys.F5)) { alerts.Clear(); lastCount = 0; lastAddress = 0; unknownChest = false; }
            Memory memory = GameController.Memory;
            hasCorruptedArea = Settings.AreaTextColor;
            int pFileRoot = memory.ReadInt(memory.AddressOfProcess + memory.offsets.FileRoot);
            int count = memory.ReadInt(pFileRoot + 0xC);
            if (count > lastCount)
            {
                int areaChangeCount = GameController.Game.AreaChangeCount;
                var listIterator = lastAddress == 0 ? memory.ReadInt(pFileRoot + 0x10) : lastAddress;
                for (int i = 0; i < count; i++)
                {
                    listIterator = memory.ReadInt(listIterator);
                    if (listIterator == 0)
                    {
                        alerts.Clear(); lastCount = 0; lastAddress = 0; return;
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
            if (alertStrings.ContainsKey(text)) { alerts.Add(alertStrings[text]); return; }

            if (text.Contains("human_heart") || text.Contains("Demonic_NoRain.ogg"))
            {
                if (Settings.CorruptedTitle) { hasCorruptedArea = Settings.HasCorruptedArea; }
                else { alerts.Add(new PreloadConfigLine { Text = "Corrupted Area", FastColor = () => Settings.HasCorruptedArea }); }
                return;
            }

            Dictionary<string, PreloadConfigLine> Labyrinth = new Dictionary<string, PreloadConfigLine>
            {

                {"Metadata/Chests/Labyrinth/LabyrinthTrinketChest", new PreloadConfigLine { Text = "Decorative Chest", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthGenericChestNoFlag", new PreloadConfigLine { Text = "Hidden Coffer", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthGenericChestTier0", new PreloadConfigLine { Text = "Hidden Coffer", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthGenericChestTier1", new PreloadConfigLine { Text = "Supply Cache", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthGenericChestTier2", new PreloadConfigLine { Text = "Labyrinth Trove", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthGenericChestTier3", new PreloadConfigLine { Text = "Ascendant's Treasures", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthGenericChestTier4", new PreloadConfigLine { Text = "Emperor's Gifts", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthWeaponsAndCurrency1", new PreloadConfigLine { Text = "Battle Supplies", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthWeaponsAndCurrency2", new PreloadConfigLine { Text = "War Spoils", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthWeaponsAndCurrency3", new PreloadConfigLine { Text = "Warmonger's Cache", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthArmourAndCurrency1", new PreloadConfigLine { Text = "Guard Supplies", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthArmourAndCurrency2", new PreloadConfigLine { Text = "Guard Vault", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthArmourAndCurrency3", new PreloadConfigLine { Text = "Guard Treasury", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthJewelryAndCurrency1", new PreloadConfigLine { Text = "Emperor's Trove", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthJewelryAndCurrency2", new PreloadConfigLine { Text = "Emperor's Vault", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthJewelryAndCurrency3", new PreloadConfigLine { Text = "Emperor's Treasury", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthMaps1", new PreloadConfigLine { Text = "Emperor's Charts", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthTreasureKey", new PreloadConfigLine { Text = "Curious Lockbox", FastColor = () => Settings.LabyrinthChests }},
                {"Metadata/Chests/Labyrinth/LabyrinthSpecificUnique", new PreloadConfigLine { Text = "Intricate Locker", FastColor = () => Settings.LabyrinthChests }}
            };
            PreloadConfigLine labyrinth_alert = Labyrinth.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (labyrinth_alert != null && alerts.Add(labyrinth_alert) && Settings.Labyrinth)
            {
                alerts.Add(labyrinth_alert);
                return;
            }

            Dictionary<string, PreloadConfigLine> PerandusLeague = new Dictionary<string, PreloadConfigLine>
            {
                {"Metadata/NPC/League/Cadiro", new PreloadConfigLine { Text = "Cadiro Trader", FastColor = () => Settings.CadiroTrader }},

                {"Metadata/Chests/PerandusChests/PerandusChestStandard", new PreloadConfigLine { Text = "Perandus Chest", FastColor = () => Settings.PerandusChestStandard }},
                {"Metadata/Chests/PerandusChests/PerandusChestRarity", new PreloadConfigLine { Text = "Perandus Cache", FastColor = () => Settings.PerandusChestRarity }},
                {"Metadata/Chests/PerandusChests/PerandusChestQuantity", new PreloadConfigLine { Text = "Perandus Hoard", FastColor = () => Settings.PerandusChestQuantity }},
                {"Metadata/Chests/PerandusChests/PerandusChestCoins", new PreloadConfigLine { Text = "Perandus Coffer", FastColor = () => Settings.PerandusChestCoins }},
                {"Metadata/Chests/PerandusChests/PerandusChestJewellery", new PreloadConfigLine { Text = "Perandus Jewellery Box", FastColor = () => Settings.PerandusChestJewellery }},
                {"Metadata/Chests/PerandusChests/PerandusChestGems", new PreloadConfigLine { Text = "Perandus Safe", FastColor = () => Settings.PerandusChestGems }},
                {"Metadata/Chests/PerandusChests/PerandusChestCurrency", new PreloadConfigLine { Text = "Perandus Treasury", FastColor = () => Settings.PerandusChestCurrency }},
                {"Metadata/Chests/PerandusChests/PerandusChestInventory", new PreloadConfigLine { Text = "Perandus Wardrobe", FastColor = () => Settings.PerandusChestInventory }},
                {"Metadata/Chests/PerandusChests/PerandusChestDivinationCards", new PreloadConfigLine { Text = "Perandus Catalogue", FastColor = () => Settings.PerandusChestDivinationCards }},
                {"Metadata/Chests/PerandusChests/PerandusChestKeepersOfTheTrove", new PreloadConfigLine { Text = "Perandus Trove", FastColor = () => Settings.PerandusChestKeepersOfTheTrove }},
                {"Metadata/Chests/PerandusChests/PerandusChestUniqueItem", new PreloadConfigLine { Text = "Perandus Locker", FastColor = () => Settings.PerandusChestUniqueItem }},
                {"Metadata/Chests/PerandusChests/PerandusChestMaps", new PreloadConfigLine { Text = "Perandus Archive", FastColor = () => Settings.PerandusChestMaps }},
                {"Metadata/Chests/PerandusChests/PerandusChestFishing", new PreloadConfigLine { Text = "Perandus Tackle Box", FastColor = () => Settings.PerandusChestFishing }},
                {"Metadata/Chests/PerandusChests/PerandusManorUniqueChest", new PreloadConfigLine { Text = "Cadiro's Locker", FastColor = () => Settings.PerandusManorUniqueChest }},
                {"Metadata/Chests/PerandusChests/PerandusManorCurrencyChest", new PreloadConfigLine { Text = "Cadiro's Treasury", FastColor = () => Settings.PerandusManorCurrencyChest }},
                {"Metadata/Chests/PerandusChests/PerandusManorMapsChest", new PreloadConfigLine { Text = "Cadiro's Archive", FastColor = () => Settings.PerandusManorMapsChest }},
                {"Metadata/Chests/PerandusChests/PerandusManorJewelryChest", new PreloadConfigLine { Text = "Cadiro's Jewellery Box", FastColor = () => Settings.PerandusManorJewelryChest }},
                {"Metadata/Chests/PerandusChests/PerandusManorDivinationCardsChest", new PreloadConfigLine { Text = "Cadiro's Catalogue", FastColor = () => Settings.PerandusManorDivinationCardsChest }},
                {"Metadata/Chests/PerandusChests/PerandusManorLostTreasureChest", new PreloadConfigLine { Text = "Grand Perandus Vault", FastColor = () => Settings.PerandusManorLostTreasureChest }},

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
            PreloadConfigLine perandus_alert = PerandusLeague.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (perandus_alert != null && Settings.PerandusLeague)
            {
                unknownChest = true;
                if (alerts.Contains(new PreloadConfigLine { Text = "Identifying chests...", FastColor = () => Settings.PerandusChestStandard }))
                {
                    alerts.Remove(new PreloadConfigLine { Text = "Identifying chests...", FastColor = () => Settings.PerandusChestStandard });
                }
                alerts.Add(perandus_alert);
                return;
            }
            if (Settings.PerandusLeague && !unknownChest && text.StartsWith("Metadata/Chests/PerandusChests/PerandusChest.ao"))
            {
                alerts.Add(new PreloadConfigLine { Text = "Identifying chests...", FastColor = () => Settings.PerandusChestStandard });
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
                {"Metadata/Chests/CopperChests/CopperChestEpic3", new PreloadConfigLine { Text = "Epic Chest", FastColor = () => Settings.EpicStrongbox }},
                {"Metadata/Chests/StrongBoxes/PerandusBox", new PreloadConfigLine { Text = "Perandus Strongbox", FastColor = () => Settings.PerandusStrongbox }},
                {"Metadata/Chests/StrongBoxes/KaomBox", new PreloadConfigLine { Text = "Kaom Strongbox", FastColor = () => Settings.KaomStrongbox }},
                {"Metadata/Chests/StrongBoxes/MalachaisBox", new PreloadConfigLine { Text = "Malachai Strongbox", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Spiders/SpiderBarrelOfSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox I", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/BarrelOfSpidersDaemonNormal2", new PreloadConfigLine { Text = "Unique Strongbox II", FastColor = () => Settings.MalachaiStrongbox }},
                {"Metadata/Monsters/Daemon/BossDaemonBarrelSpidersBoss", new PreloadConfigLine { Text = "Unique Strongbox III", FastColor = () => Settings.MalachaiStrongbox }}
            };
            PreloadConfigLine strongboxes_alert = Strongboxes.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (strongboxes_alert != null && alerts.Add(strongboxes_alert) && Settings.Strongboxes)
            {
                alerts.Add(strongboxes_alert);
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
            PreloadConfigLine masters_alert = Masters.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (masters_alert != null && alerts.Add(masters_alert) && Settings.Masters)
            {
                alerts.Add(masters_alert);
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
            PreloadConfigLine exiles_alert = Exiles.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).FirstOrDefault();
            if (exiles_alert != null && alerts.Add(exiles_alert) && Settings.Exiles)
            {
                alerts.Add(exiles_alert);
                return;
            }
        }
    }
}