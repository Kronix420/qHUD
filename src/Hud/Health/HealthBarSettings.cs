namespace qHUD.Hud.Health
{
    using Settings;
    public sealed class HealthBarSettings : SettingsBase
    {
        public HealthBarSettings()
        {
            Enable = true;
            ShowInTown = false;
            ShowES = true;
            ShowIncrements = true;
            ShowEnemies = true;
            Players = new UnitSettings(0x008000ff, 0);
            Minions = new UnitSettings(0x90ee90ff, 0);
            NormalEnemy = new UnitSettings(0xff0000ff, 0);
            MagicEnemy = new UnitSettings(0xff0000ff, 0x8888ffff);
            RareEnemy = new UnitSettings(0xff0000ff, 0xffff77ff);
            UniqueEnemy = new UnitSettings(0xff0000ff, 0xffa500ff);
            xPlayers = new RangeNode<int>(100, 0, 200);
            yPlayers = new RangeNode<int>(100, 0, 200);
            xEnemies = new RangeNode<int>(50, 0, 200);
            yEnemies = new RangeNode<int>(50, 0, 200);
        }

        public ToggleNode ShowInTown { get; set; }
        public ToggleNode ShowES { get; set; }
        public ToggleNode ShowIncrements { get; set; }
        public ToggleNode ShowEnemies { get; set; }
        public UnitSettings Players { get; set; }
        public UnitSettings Minions { get; set; }
        public UnitSettings NormalEnemy { get; set; }
        public UnitSettings MagicEnemy { get; set; }
        public UnitSettings RareEnemy { get; set; }
        public UnitSettings UniqueEnemy { get; set; }
        public RangeNode<int> xPlayers { get; set; }
        public RangeNode<int> yPlayers { get; set; }
        public RangeNode<int> xEnemies { get; set; }
        public RangeNode<int> yEnemies { get; set; }
    }
}