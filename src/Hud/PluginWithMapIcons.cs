namespace qHUD.Hud
{
    using System.Collections.Generic;
    using Controllers;
    using Interfaces;
    using Settings;
    using UI;
    using Models;

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