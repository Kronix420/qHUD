using System;
using System.Linq;
using System.Windows.Forms;
using qHUD.Controllers;
using qHUD.Framework;
using qHUD.Framework.InputHooks;
using qHUD.Hud.Health;
using qHUD.Hud.Settings;
using qHUD.Hud.UI;
using SharpDX;

namespace qHUD.Hud.Menu
{
    public class MenuPlugin : Plugin<MenuSettings>
    {
        private readonly SettingsHub settingsHub;
        private readonly Action<MouseInfo> onMouseDown, onMouseUp, onMouseMove;
        private bool holdKey;
        private RootButton root;

        public MenuPlugin(GameController gameController, Graphics graphics, SettingsHub settingsHub)
            : base(gameController, graphics, settingsHub.MenuSettings)
        {
            this.settingsHub = settingsHub;
            CreateMenu();
            MouseHook.MouseDown += onMouseDown = info => info.Handled = OnMouseEvent(MouseEventID.LeftButtonDown, info.Position);
            MouseHook.MouseUp += onMouseUp = info => info.Handled = OnMouseEvent(MouseEventID.LeftButtonUp, info.Position);
            MouseHook.MouseMove += onMouseMove = info => info.Handled = OnMouseEvent(MouseEventID.MouseMove, info.Position);
        }

        public override void Dispose()
        {
            MouseHook.MouseDown -= onMouseDown;
            MouseHook.MouseUp -= onMouseUp;
            MouseHook.MouseMove -= onMouseMove;
        }

        public override void Render()
        {
            if (!holdKey && WinApi.IsKeyDown(Keys.F12))
            {
                holdKey = true;
                Settings.Enable.Value = !Settings.Enable.Value;
                SettingsHub.Save(settingsHub);
            }
            else if (holdKey && !WinApi.IsKeyDown(Keys.F12))
            {
                holdKey = false;
            }

            if (Settings.Enable)
            {
                root.Render(Graphics, Settings);
            }
        }

        private static MenuItem AddChild(MenuItem parent, string text, ToggleNode node, string key = null, Func<MenuItem, bool> hide = null)
        {
            var item = new ToggleButton(parent, text, node, key, hide);
            parent.AddChild(item);
            return item;
        }

        private static void AddChild(MenuItem parent, FileNode path)
        {
            var item = new FileButton(path);
            parent.AddChild(item);
        }

        private static void AddChild(MenuItem parent, string text, ColorNode node)
        {
            var item = new ColorButton(text, node);
            parent.AddChild(item);
        }

        private static void AddChild<T>(MenuItem parent, string text, RangeNode<T> node) where T : struct
        {
            var item = new Picker<T>(text, node);
            parent.AddChild(item);
        }

        private void CreateMenu()
        {
            root = new RootButton(new Vector2(Settings.X, Settings.Y));

            // Health bars
            HealthBarSettings healthBarPlugin = settingsHub.HealthBarSettings;
            MenuItem healthMenu = AddChild(root, "Health bars", healthBarPlugin.Enable);
            MenuItem playersMenu = AddChild(healthMenu, "Players", healthBarPlugin.Players.Enable);
            MenuItem enemiesMenu = AddChild(healthMenu, "Enemies", healthBarPlugin.ShowEnemies);
            MenuItem minionsMenu = AddChild(healthMenu, "Minions", healthBarPlugin.Minions.Enable);
            AddChild(healthMenu, "Show nrg shield", healthBarPlugin.ShowES);
            AddChild(healthMenu, "Show in town", healthBarPlugin.ShowInTown);
            AddChild(playersMenu, "Width", healthBarPlugin.Players.Width);
            AddChild(playersMenu, "Height", healthBarPlugin.Players.Height);
            AddChild(playersMenu, "Position X", settingsHub.HealthBarSettings.xPlayers);
            AddChild(playersMenu, "Position Y", settingsHub.HealthBarSettings.yPlayers);
            AddChild(minionsMenu, "Width", healthBarPlugin.Minions.Width);
            AddChild(minionsMenu, "Height", healthBarPlugin.Minions.Height);
            MenuItem whiteEnemyMenu = AddChild(enemiesMenu, "White", healthBarPlugin.NormalEnemy.Enable);
            AddChild(whiteEnemyMenu, "Width", healthBarPlugin.NormalEnemy.Width);
            AddChild(whiteEnemyMenu, "Height", healthBarPlugin.NormalEnemy.Height);
            MenuItem magicEnemyMenu = AddChild(enemiesMenu, "Magic", healthBarPlugin.MagicEnemy.Enable);
            AddChild(magicEnemyMenu, "Width", healthBarPlugin.MagicEnemy.Width);
            AddChild(magicEnemyMenu, "Height", healthBarPlugin.MagicEnemy.Height);
            MenuItem rareEnemyMenu = AddChild(enemiesMenu, "Rare", healthBarPlugin.RareEnemy.Enable);
            AddChild(rareEnemyMenu, "Width", healthBarPlugin.RareEnemy.Width);
            AddChild(rareEnemyMenu, "Height", healthBarPlugin.RareEnemy.Height);
            MenuItem uniquesEnemyMenu = AddChild(enemiesMenu, "Uniques", healthBarPlugin.UniqueEnemy.Enable);
            AddChild(uniquesEnemyMenu, "Width", healthBarPlugin.UniqueEnemy.Width);
            AddChild(uniquesEnemyMenu, "Height", healthBarPlugin.UniqueEnemy.Height);
            AddChild(enemiesMenu, "Position X", settingsHub.HealthBarSettings.xEnemies);
            AddChild(enemiesMenu, "Position Y", settingsHub.HealthBarSettings.yEnemies);

            // Area display
            MenuItem areaName = AddChild(root, "Area title", settingsHub.AreaSettings.Enable, "F10");
            AddChild(areaName, "Show latency", settingsHub.AreaSettings.ShowLatency);
            AddChild(areaName, "Show in town", settingsHub.AreaSettings.ShowInTown);
            AddChild(areaName, "Font size", settingsHub.AreaSettings.TextSize);
            AddChild(areaName, "Area font color", settingsHub.PreloadAlertSettings.AreaTextColor);
            AddChild(areaName, "Latency font color", settingsHub.AreaSettings.LatencyTextColor);
            AddChild(areaName, "Background color", settingsHub.AreaSettings.BackgroundColor);

            // Item alert
            MenuItem itemAlertMenu = AddChild(root, "Item alert", settingsHub.ItemAlertSettings.Enable);
            var itemAlertStaticMenuList = new[] { "Choose filter", "Item tooltips", "Play sound", "Show text" };
            MenuItem alternative = AddChild(itemAlertMenu, itemAlertStaticMenuList[0], settingsHub.ItemAlertSettings.Alternative, null, y => itemAlertStaticMenuList.All(x => x != (y as ToggleButton)?.Name));
            AddChild(alternative, settingsHub.ItemAlertSettings.FilePath);
            AddChild(alternative, "With border", settingsHub.ItemAlertSettings.WithBorder);
            AddChild(alternative, "With sound", settingsHub.ItemAlertSettings.WithSound);
            MenuItem tooltipMenu = AddChild(itemAlertMenu, itemAlertStaticMenuList[1], settingsHub.ItemTooltipSettings.Enable);
            MenuItem itemLevelMenu = AddChild(tooltipMenu, "Item level", settingsHub.ItemTooltipSettings.ItemLevel.Enable);
            AddChild(itemLevelMenu, "Text size", settingsHub.ItemTooltipSettings.ItemLevel.TextSize);
            AddChild(itemLevelMenu, "Text color", settingsHub.ItemTooltipSettings.ItemLevel.TextColor);
            AddChild(itemLevelMenu, "Background color", settingsHub.ItemTooltipSettings.ItemLevel.BackgroundColor);
            MenuItem itemModsMenu = AddChild(tooltipMenu, "Item mods", settingsHub.ItemTooltipSettings.ItemMods.Enable, "F9");
            AddChild(itemModsMenu, "Tier 1 size", settingsHub.ItemTooltipSettings.ItemMods.TierTextSize);
            AddChild(itemModsMenu, "Mods size", settingsHub.ItemTooltipSettings.ItemMods.ModTextSize);
            AddChild(itemModsMenu, "Tier 1 color", settingsHub.ItemTooltipSettings.ItemMods.T1Color);
            AddChild(itemModsMenu, "Tier 2 color", settingsHub.ItemTooltipSettings.ItemMods.T2Color);
            AddChild(itemModsMenu, "Tier 3 color", settingsHub.ItemTooltipSettings.ItemMods.T3Color);
            AddChild(itemModsMenu, "Suffix color", settingsHub.ItemTooltipSettings.ItemMods.SuffixColor);
            AddChild(itemModsMenu, "Prefix color", settingsHub.ItemTooltipSettings.ItemMods.PrefixColor);
            MenuItem weaponDpsMenu = AddChild(tooltipMenu, "Weapon dps", settingsHub.ItemTooltipSettings.WeaponDps.Enable);
            var damageColors = AddChild(weaponDpsMenu, "Damage colors", settingsHub.ItemTooltipSettings.WeaponDps.Enable);
            AddChild(damageColors, "Cold damage", settingsHub.ItemTooltipSettings.WeaponDps.DmgColdColor);
            AddChild(damageColors, "Fire damage", settingsHub.ItemTooltipSettings.WeaponDps.DmgFireColor);
            AddChild(damageColors, "Lightning damage", settingsHub.ItemTooltipSettings.WeaponDps.DmgLightningColor);
            AddChild(damageColors, "Chaos damage", settingsHub.ItemTooltipSettings.WeaponDps.DmgChaosColor);
            AddChild(damageColors, "Physical damage", settingsHub.ItemTooltipSettings.WeaponDps.pDamageColor);
            AddChild(damageColors, "Elemental damage", settingsHub.ItemTooltipSettings.WeaponDps.eDamageColor);
            AddChild(weaponDpsMenu, "Dps size", settingsHub.ItemTooltipSettings.WeaponDps.DpsTextSize);
            AddChild(weaponDpsMenu, "Dps text size", settingsHub.ItemTooltipSettings.WeaponDps.DpsNameTextSize);
            AddChild(weaponDpsMenu, "Background color", settingsHub.ItemTooltipSettings.WeaponDps.BackgroundColor);
            MenuItem itemSound = AddChild(itemAlertMenu, itemAlertStaticMenuList[2], settingsHub.ItemAlertSettings.PlaySound);
            AddChild(itemSound, "Sound volume", settingsHub.ItemAlertSettings.SoundVolume);
            MenuItem alertTextMenu = AddChild(itemAlertMenu, itemAlertStaticMenuList[3], settingsHub.ItemAlertSettings.ShowText);
            AddChild(alertTextMenu, "Text size", settingsHub.ItemAlertSettings.TextSize);

            // Preload alert
            var preloadMenu = AddChild(root, "Preload alert", settingsHub.PreloadAlertSettings.Enable, "F5");

            var perandus = AddChild(preloadMenu, "Perandus League", settingsHub.PreloadAlertSettings.PerandusLeague);
            AddChild(perandus, "Cadiro Trader", settingsHub.PreloadAlertSettings.CadiroTrader);
            AddChild(perandus, "Perandus Locker", settingsHub.PreloadAlertSettings.PerandusChestUniqueItem);
            AddChild(perandus, "Perandus Treasury", settingsHub.PreloadAlertSettings.PerandusChestCurrency);
            AddChild(perandus, "Perandus Catalogue", settingsHub.PreloadAlertSettings.PerandusChestDivinationCards);
            AddChild(perandus, "Perandus Archive", settingsHub.PreloadAlertSettings.PerandusChestMaps);
            AddChild(perandus, "Perandus Tackle Box", settingsHub.PreloadAlertSettings.PerandusChestFishing);
            AddChild(perandus, "Perandus Coffer", settingsHub.PreloadAlertSettings.PerandusChestCoins);
            AddChild(perandus, "Perandus Chest", settingsHub.PreloadAlertSettings.PerandusChestStandard);
            AddChild(perandus, "Perandus Trove", settingsHub.PreloadAlertSettings.PerandusChestKeepersOfTheTrove);
            AddChild(perandus, "Perandus Cache", settingsHub.PreloadAlertSettings.PerandusChestRarity);
            AddChild(perandus, "Perandus Hoard", settingsHub.PreloadAlertSettings.PerandusChestQuantity);
            AddChild(perandus, "Perandus Jewellery", settingsHub.PreloadAlertSettings.PerandusChestJewellery);
            AddChild(perandus, "Perandus Safe", settingsHub.PreloadAlertSettings.PerandusChestGems);
            AddChild(perandus, "Perandus Wardrobe", settingsHub.PreloadAlertSettings.PerandusChestInventory);
            AddChild(perandus, "Cadiro's Locker", settingsHub.PreloadAlertSettings.PerandusManorUniqueChest);
            AddChild(perandus, "Cadiro's Treasury", settingsHub.PreloadAlertSettings.PerandusManorCurrencyChest);
            AddChild(perandus, "Cadiro's Catalogue", settingsHub.PreloadAlertSettings.PerandusManorDivinationCardsChest);
            AddChild(perandus, "Cadiro's Archive", settingsHub.PreloadAlertSettings.PerandusManorMapsChest);
            AddChild(perandus, "Cadiro's Jewellery", settingsHub.PreloadAlertSettings.PerandusManorJewelryChest);
            AddChild(perandus, "Grand Perandus Vault", settingsHub.PreloadAlertSettings.PerandusManorLostTreasureChest);
            AddChild(perandus, "Perandus Guards", settingsHub.PreloadAlertSettings.PerandusGuards);
            AddChild(perandus, "Labyrinth Chests", settingsHub.PreloadAlertSettings.LabyrinthChests);

            var masters = AddChild(preloadMenu, "Masters", settingsHub.PreloadAlertSettings.Masters);
            AddChild(masters, "Zana", settingsHub.PreloadAlertSettings.MasterZana);
            AddChild(masters, "Krillson", settingsHub.PreloadAlertSettings.MasterKrillson);
            AddChild(masters, "Tora", settingsHub.PreloadAlertSettings.MasterTora);
            AddChild(masters, "Haku", settingsHub.PreloadAlertSettings.MasterHaku);
            AddChild(masters, "Vorici", settingsHub.PreloadAlertSettings.MasterVorici);
            AddChild(masters, "Elreon", settingsHub.PreloadAlertSettings.MasterElreon);
            AddChild(masters, "Vagan", settingsHub.PreloadAlertSettings.MasterVagan);
            AddChild(masters, "Catarina", settingsHub.PreloadAlertSettings.MasterCatarina);

            var exiles = AddChild(preloadMenu, "Exiles", settingsHub.PreloadAlertSettings.Exiles);
            AddChild(exiles, "Orra Greengate", settingsHub.PreloadAlertSettings.OrraGreengate);
            AddChild(exiles, "Thena Moga", settingsHub.PreloadAlertSettings.ThenaMoga);
            AddChild(exiles, "Antalie Napora", settingsHub.PreloadAlertSettings.AntalieNapora);
            AddChild(exiles, "Torr Olgosso", settingsHub.PreloadAlertSettings.TorrOlgosso);
            AddChild(exiles, "Armios Bell", settingsHub.PreloadAlertSettings.ArmiosBell);
            AddChild(exiles, "Zacharie Desmarais", settingsHub.PreloadAlertSettings.ZacharieDesmarais);
            AddChild(exiles, "Minara Anenima", settingsHub.PreloadAlertSettings.MinaraAnenima);
            AddChild(exiles, "Igna Phoenix", settingsHub.PreloadAlertSettings.IgnaPhoenix);
            AddChild(exiles, "Jonah Unchained", settingsHub.PreloadAlertSettings.JonahUnchained);
            AddChild(exiles, "Damoi Tui", settingsHub.PreloadAlertSettings.DamoiTui);
            AddChild(exiles, "Xandro Blooddrinker", settingsHub.PreloadAlertSettings.XandroBlooddrinker);
            AddChild(exiles, "Vickas Giantbone", settingsHub.PreloadAlertSettings.VickasGiantbone);
            AddChild(exiles, "Eoin Greyfur", settingsHub.PreloadAlertSettings.EoinGreyfur);
            AddChild(exiles, "Tinevin Highdove", settingsHub.PreloadAlertSettings.TinevinHighdove);
            AddChild(exiles, "Magnus Stonethorn", settingsHub.PreloadAlertSettings.MagnusStonethorn);
            AddChild(exiles, "Ion Darkshroud", settingsHub.PreloadAlertSettings.IonDarkshroud);
            AddChild(exiles, "Ash Lessard", settingsHub.PreloadAlertSettings.AshLessard);
            AddChild(exiles, "Wilorin Demontamer", settingsHub.PreloadAlertSettings.WilorinDemontamer);
            AddChild(exiles, "Augustina Solaria", settingsHub.PreloadAlertSettings.AugustinaSolaria);
            AddChild(exiles, "Dena Lorenni", settingsHub.PreloadAlertSettings.DenaLorenni);
            AddChild(exiles, "Vanth Agiel", settingsHub.PreloadAlertSettings.VanthAgiel);
            AddChild(exiles, "Lael Furia", settingsHub.PreloadAlertSettings.LaelFuria);

            var tormented = AddChild(preloadMenu, "Tormented ghosts", settingsHub.PreloadAlertSettings.Tormented);
            AddChild(tormented, "Embezzler", settingsHub.PreloadAlertSettings.TormentedEmbezzler);
            AddChild(tormented, "Seditionist", settingsHub.PreloadAlertSettings.TormentedLibrarian);
            AddChild(tormented, "Smuggler", settingsHub.PreloadAlertSettings.TormentedSmuggler);
            AddChild(tormented, "Arsonist", settingsHub.PreloadAlertSettings.TormentedArsonist);
            AddChild(tormented, "Aurora Cultist", settingsHub.PreloadAlertSettings.TormentedFreezer);
            AddChild(tormented, "Blasphemer", settingsHub.PreloadAlertSettings.TormentedBlasphemer);
            AddChild(tormented, "Cannibal", settingsHub.PreloadAlertSettings.TormentedCannibal);
            AddChild(tormented, "Charlatan", settingsHub.PreloadAlertSettings.TormentedCharlatan);
            AddChild(tormented, "Cutthroat", settingsHub.PreloadAlertSettings.TormentedCutthroat);
            AddChild(tormented, "Forger", settingsHub.PreloadAlertSettings.TormentedCounterfeiter);
            AddChild(tormented, "Martyr", settingsHub.PreloadAlertSettings.TormentedMartyr);
            AddChild(tormented, "Mutilator", settingsHub.PreloadAlertSettings.TormentedMutilator);
            AddChild(tormented, "Necromancer", settingsHub.PreloadAlertSettings.TormentedNecromancer);
            AddChild(tormented, "Poisoner", settingsHub.PreloadAlertSettings.TormentedPoisoner);
            AddChild(tormented, "Rogue", settingsHub.PreloadAlertSettings.TormentedRogue);
            AddChild(tormented, "Spy", settingsHub.PreloadAlertSettings.TormentedSpy);
            AddChild(tormented, "Storm Cultist", settingsHub.PreloadAlertSettings.TormentedExperimenter);
            AddChild(tormented, "Thief", settingsHub.PreloadAlertSettings.TormentedThief);
            AddChild(tormented, "Thug", settingsHub.PreloadAlertSettings.TormentedThug);
            AddChild(tormented, "Vaal Cultist", settingsHub.PreloadAlertSettings.TormentedCorrupter);
            AddChild(tormented, "Warlord", settingsHub.PreloadAlertSettings.TormentedWarlord);
            AddChild(tormented, "Fisherman", settingsHub.PreloadAlertSettings.TormentedFisherman);

            var strongboxes = AddChild(preloadMenu, "Strongboxes", settingsHub.PreloadAlertSettings.Strongboxes);
            AddChild(strongboxes, "Arcanist", settingsHub.PreloadAlertSettings.ArcanistStrongbox);
            AddChild(strongboxes, "Diviners", settingsHub.PreloadAlertSettings.DivinersStrongbox);
            AddChild(strongboxes, "Cartographer", settingsHub.PreloadAlertSettings.CartographerStrongbox);
            AddChild(strongboxes, "Gemcutter", settingsHub.PreloadAlertSettings.GemcutterStrongbox);
            AddChild(strongboxes, "Artisan", settingsHub.PreloadAlertSettings.ArtisanStrongbox);
            AddChild(strongboxes, "Jeweller", settingsHub.PreloadAlertSettings.JewellerStrongbox);
            AddChild(strongboxes, "Blacksmith", settingsHub.PreloadAlertSettings.BlacksmithStrongbox);
            AddChild(strongboxes, "Armourer", settingsHub.PreloadAlertSettings.ArmourerStrongbox);
            AddChild(strongboxes, "Ornate", settingsHub.PreloadAlertSettings.OrnateStrongbox);
            AddChild(strongboxes, "Large", settingsHub.PreloadAlertSettings.LargeStrongbox);
            AddChild(strongboxes, "Perandus", settingsHub.PreloadAlertSettings.PerandusStrongbox);
            AddChild(strongboxes, "Kaom", settingsHub.PreloadAlertSettings.KaomStrongbox);
            AddChild(strongboxes, "Malachai", settingsHub.PreloadAlertSettings.MalachaiStrongbox);
            AddChild(strongboxes, "Epic", settingsHub.PreloadAlertSettings.EpicStrongbox);
            AddChild(strongboxes, "Simple", settingsHub.PreloadAlertSettings.SimpleStrongbox);
            var corruptedMenu = AddChild(preloadMenu, "Corrupted Area", settingsHub.PreloadAlertSettings.Strongboxes);
            AddChild(corruptedMenu, "Corrupted title", settingsHub.AreaSettings.CorruptedTitle);
            AddChild(preloadMenu, "Background color", settingsHub.PreloadAlertSettings.BackgroundColor);
            AddChild(preloadMenu, "Font color", settingsHub.PreloadAlertSettings.DefaultTextColor);
            AddChild(preloadMenu, "Font size", settingsHub.PreloadAlertSettings.TextSize);

            AddChild(preloadMenu, "Position X", settingsHub.PreloadAlertSettings.TextPositionX);
            AddChild(preloadMenu, "Position Y", settingsHub.PreloadAlertSettings.TextPositionY);

            // Monster alert
            MenuItem MonsterTrackerMenu = AddChild(root, "Monster alert", settingsHub.MonsterTrackerSettings.Enable);
            MenuItem alertSound = AddChild(MonsterTrackerMenu, "Sound warning", settingsHub.MonsterTrackerSettings.PlaySound);
            AddChild(alertSound, "Sound volume", settingsHub.MonsterTrackerSettings.SoundVolume);
            MenuItem warningTextMenu = AddChild(MonsterTrackerMenu, "Text warning", settingsHub.MonsterTrackerSettings.ShowText);
            AddChild(warningTextMenu, "Text size", settingsHub.MonsterTrackerSettings.TextSize);
            AddChild(warningTextMenu, "Default text color:", settingsHub.MonsterTrackerSettings.DefaultTextColor);
            AddChild(warningTextMenu, "Background color:", settingsHub.MonsterTrackerSettings.BackgroundColor);
            AddChild(warningTextMenu, "Position X", settingsHub.MonsterTrackerSettings.TextPositionX);
            AddChild(warningTextMenu, "Position Y", settingsHub.MonsterTrackerSettings.TextPositionY);

            // Map icons
            MenuItem minimapIcons = AddChild(root, "Minimap icons", settingsHub.MapIconsSettings.Enable);
            AddChild(minimapIcons, "White Mob Icon", settingsHub.MonsterTrackerSettings.WhiteMobIcon);
            AddChild(minimapIcons, "Magic Mob Icon", settingsHub.MonsterTrackerSettings.MagicMobIcon);
            AddChild(minimapIcons, "Rare Mob Icon", settingsHub.MonsterTrackerSettings.RareMobIcon);
            AddChild(minimapIcons, "Unique Mob Icon", settingsHub.MonsterTrackerSettings.UniqueMobIcon);
            AddChild(minimapIcons, "Minions Icon", settingsHub.MonsterTrackerSettings.MinionsIcon);
            AddChild(minimapIcons, "Masters Icon", settingsHub.PoiTrackerSettings.MastersIcon);
            AddChild(minimapIcons, "Chests Icon", settingsHub.PoiTrackerSettings.ChestsIcon);
            AddChild(minimapIcons, "Strongboxes Icon", settingsHub.PoiTrackerSettings.StrongboxesIcon);
            AddChild(minimapIcons, "PerandusChest Icon", settingsHub.PoiTrackerSettings.PerandusChestIcon);
            AddChild(minimapIcons, "Item loot Icon", settingsHub.ItemAlertSettings.LootIcon);

            //Menu settings
            var menuSettings = AddChild(root, "Menu settings", settingsHub.MenuSettings.ShowMenu, "F12");
            AddChild(menuSettings, "Menu font color", settingsHub.MenuSettings.MenuFontColor);
            AddChild(menuSettings, "Title font color", settingsHub.MenuSettings.TitleFontColor);
            AddChild(menuSettings, "Enabled color", settingsHub.MenuSettings.EnabledBoxColor);
            AddChild(menuSettings, "Disabled color", settingsHub.MenuSettings.DisabledBoxColor);
            AddChild(menuSettings, "Slider color", settingsHub.MenuSettings.SliderColor);
            AddChild(menuSettings, "Background color", settingsHub.MenuSettings.BackgroundColor);
            AddChild(menuSettings, "Menu font size", settingsHub.MenuSettings.MenuFontSize);
            AddChild(menuSettings, "Title font size", settingsHub.MenuSettings.TitleFontSize);
            AddChild(menuSettings, "Picker font size", settingsHub.MenuSettings.PickerFontSize);
        }

        private bool OnMouseEvent(MouseEventID id, Point position)
        {
            if (!Settings.Enable || !GameController.Window.IsForeground())
            {
                return false;
            }

            Vector2 mousePosition = GameController.Window.ScreenToClient(position.X, position.Y);
            return root.OnMouseEvent(id, mousePosition);
        }
    }
}