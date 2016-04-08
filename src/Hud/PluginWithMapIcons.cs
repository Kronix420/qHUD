using System.Collections.Generic;
using qHUD.Controllers;
using qHUD.Hud.Interfaces;
using qHUD.Hud.Settings;
using qHUD.Hud.UI;
using qHUD.Models;

namespace qHUD.Hud
{
    public abstract class PluginWithMapIcons<TSettings> : Plugin<TSettings>, IPluginWithMapIcons where TSettings : SettingsBase
    {
        protected readonly Dictionary<EntityWrapper, MapIcon> CurrentIcons;

        protected PluginWithMapIcons(GameController gameController, Graphics graphics, TSettings settings) : base(gameController, graphics, settings)
        {
            CurrentIcons = new Dictionary<EntityWrapper, MapIcon>();
            GameController.Area.OnAreaChange += delegate
             {
                 CurrentIcons.Clear();
             };
        }

        protected override void OnEntityRemoved(EntityWrapper entityWrapper)
        {
            base.OnEntityRemoved(entityWrapper);
            CurrentIcons.Remove(entityWrapper);
        }

        public IEnumerable<MapIcon> GetIcons()
        {
            var toRemove = new List<EntityWrapper>();
            foreach (var kv in CurrentIcons)
            {
                if (kv.Value.IsEntityStillValid())
                    yield return kv.Value;
                else
                    toRemove.Add(kv.Key);
            }
            foreach (EntityWrapper wrapper in toRemove)
            {
                CurrentIcons.Remove(wrapper);
            }
        }
    }
}