namespace qHUD.Hud.Trackers
{
    using System.Collections.Generic;
    using Controllers;
    using UI;
    using Models;
    using Poe.Components;

    public class PoiTracker : PluginWithMapIcons<PoiTrackerSettings>
    {
        private static readonly List<string> masters = new List<string>
        {
            "Metadata/NPC/Missions/Wild/Dex",
            "Metadata/NPC/Missions/Wild/DexInt",
            "Metadata/NPC/Missions/Wild/Int",
            "Metadata/NPC/Missions/Wild/Str",
            "Metadata/NPC/Missions/Wild/StrDex",
            "Metadata/NPC/Missions/Wild/StrDexInt",
            "Metadata/NPC/Missions/Wild/StrInt"
        };

        public PoiTracker(GameController gameController, Graphics graphics, PoiTrackerSettings settings)
            : base(gameController, graphics, settings) { }

        public override void Render()
        {
            if (!Settings.Enable || GameController.Area.CurrentArea.IsTown || GameController.Area.CurrentArea.IsHideout) { }
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            if (!Settings.Enable || GameController.Area.CurrentArea.IsTown || GameController.Area.CurrentArea.IsHideout) { return; }

            MapIcon icon = GetMapIcon(entity);
            if (null != icon)
            {
                CurrentIcons[entity] = icon;
            }
        }

        private MapIcon GetMapIcon(EntityWrapper e)
        {
            if (e.HasComponent<NPC>() && masters.Contains(e.Path))
            {
                return new CreatureMapIcon(e, "ms-cyan.png", () => Settings.Masters, Settings.MastersIcon);
            }
            if (e.HasComponent<Chest>() && !e.GetComponent<Chest>().IsOpened)
            {
                return e.GetComponent<Chest>().IsStrongbox
                    ? new ChestMapIcon(e, new HudTexture("strongbox.png",
                    e.GetComponent<ObjectMagicProperties>().Rarity), () => Settings.Strongboxes, Settings.StrongboxesIcon)
                    : new ChestMapIcon(e, new HudTexture("chest.png"), () => Settings.Chests, Settings.ChestsIcon);
            }
            return null;
        }
    }
}