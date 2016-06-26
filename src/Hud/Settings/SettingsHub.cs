namespace qHUD.Hud.Settings
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Area;
    using Health;
    using Icons;
    using ItemTooltips;
    using Loot;
    using Menu;
    using Preload;
    using Converters;
    using Trackers;
    public sealed class SettingsHub
    {
        private const string SETTINGS_FILE_NAME = "config/settings.json";

        private static readonly JsonSerializerSettings jsonSettings;

        static SettingsHub()
        {
            jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new SortContractResolver(),
                Converters = new JsonConverter[]
                {
                    new ColorNodeConverter(),
                    new ToggleNodeConverter(),
                    new FileNodeConverter()
                }
            };
        }

        public SettingsHub()
        {
            MenuSettings = new MenuSettings();
            MapIconsSettings = new MapIconsSettings();
            ItemAlertSettings = new ItemAlertSettings();
            ItemTooltipSettings = new ItemTooltipSettings();
            MonsterTrackerSettings = new MonsterTrackerSettings();
            PoiTrackerSettings = new PoiTrackerSettings();
            PreloadAlertSettings = new PreloadAlertSettings();
            AreaSettings = new AreaSettings();
            HealthBarSettings = new HealthBarSettings();
        }

        [JsonProperty("Menu")]
        public MenuSettings MenuSettings { get; private set; }

        [JsonProperty("Minimap icons")]
        public MapIconsSettings MapIconsSettings { get; private set; }

        [JsonProperty("Item alert")]
        public ItemAlertSettings ItemAlertSettings { get; private set; }

        [JsonProperty("Item tooltips settings")]
        public ItemTooltipSettings ItemTooltipSettings { get; private set; }

        [JsonProperty("Monsters tracker")]
        public MonsterTrackerSettings MonsterTrackerSettings { get; private set; }

        [JsonProperty("Masters tracker")]
        public PoiTrackerSettings PoiTrackerSettings { get; private set; }

        [JsonProperty("Preload alerts")]
        public PreloadAlertSettings PreloadAlertSettings { get; private set; }

        [JsonProperty("Area settings")]
        public AreaSettings AreaSettings { get; private set; }

        [JsonProperty("Health bar")]
        public HealthBarSettings HealthBarSettings { get; private set; }

        public static SettingsHub Load()
        {
            try
            {
                string json = File.ReadAllText(SETTINGS_FILE_NAME);
                return JsonConvert.DeserializeObject<SettingsHub>(json, jsonSettings);
            }
            catch
            {
                if (File.Exists(SETTINGS_FILE_NAME))
                {
                    string backupFileName = SETTINGS_FILE_NAME + DateTime.Now.Ticks;
                    File.Move(SETTINGS_FILE_NAME, backupFileName);
                }

                var settings = new SettingsHub();
                Save(settings);
                return settings;
            }
        }

        public static void Save(SettingsHub settings)
        {
            using (var stream = new StreamWriter(File.Create(SETTINGS_FILE_NAME)))
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented, jsonSettings);
                stream.Write(json);
            }
        }
    }
}