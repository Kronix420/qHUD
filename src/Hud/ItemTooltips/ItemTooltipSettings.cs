using Newtonsoft.Json;
using qHUD.Hud.Settings;

namespace qHUD.Hud.ItemTooltips
{
    public class ItemTooltipSettings : SettingsBase
    {
        public ItemTooltipSettings()
    {
            Enable = true;
            ItemLevel = new ItemLevelSettings();
            ItemMods = new ItemModsSettings();
            WeaponDps = new WeaponDpsSettings();
        }

        [JsonProperty("Item level")]
        public ItemLevelSettings ItemLevel { get; set; }
        [JsonProperty("Item mods")]
        public ItemModsSettings ItemMods { get; set; }
        [JsonProperty("Weapon DPS")]
        public WeaponDpsSettings WeaponDps { get; set; }
    }
}