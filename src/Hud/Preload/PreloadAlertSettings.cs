namespace qHUD.Hud.Preload
{
    using Settings;
    using SharpDX;

    public sealed class PreloadAlertSettings : SettingsBase
    {
        public PreloadAlertSettings()
        {
            Enable = true;
            Masters = true;
            Exiles = true;
            Tormented = true;
            Strongboxes = true;
            TextSize = new RangeNode<int>(16, 10, 50);
            BackgroundColor = new ColorBGRA(0, 0, 0, 255);
            DefaultTextColor = new ColorBGRA(210, 210, 210, 255);
            AreaTextColor = new ColorBGRA(150, 200, 250, 255);

            MasterZana = new ColorBGRA(255, 255, 0, 255);
            MasterCatarina = new ColorBGRA(100, 255, 255, 255);
            MasterTora = new ColorBGRA(100, 255, 255, 255);
            MasterVorici = new ColorBGRA(100, 255, 255, 255);
            MasterHaku = new ColorBGRA(100, 255, 255, 255);
            MasterElreon = new ColorBGRA(100, 255, 255, 255);
            MasterVagan = new ColorBGRA(100, 255, 255, 255);
            MasterKrillson = new ColorBGRA(255, 0, 255, 255);

            DivinersStrongbox = new ColorBGRA(255, 0, 0, 255);
            ArcanistStrongbox = new ColorBGRA(255, 0, 255, 255);
            ArtisanStrongbox = new ColorBGRA(210, 210, 210, 255);
            CartographerStrongbox = new ColorBGRA(255, 255, 0, 255);
            GemcutterStrongbox = new ColorBGRA(27, 162, 155, 255);
            JewellerStrongbox = new ColorBGRA(210, 210, 210, 255);
            BlacksmithStrongbox = new ColorBGRA(210, 210, 210, 255);
            ArmourerStrongbox = new ColorBGRA(210, 210, 210, 255);
            OrnateStrongbox = new ColorBGRA(210, 210, 210, 255);
            LargeStrongbox = new ColorBGRA(210, 210, 210, 255);
            PerandusStrongbox = new ColorBGRA(175, 96, 37, 255);
            KaomStrongbox = new ColorBGRA(175, 96, 37, 255);
            MalachaiStrongbox = new ColorBGRA(175, 96, 37, 255);
            EpicStrongbox = new ColorBGRA(175, 96, 37, 255);
            SimpleStrongbox = new ColorBGRA(210, 210, 210, 255);

            OrraGreengate = new ColorBGRA(254, 192, 118, 255);
            ThenaMoga = new ColorBGRA(254, 192, 118, 255);
            AntalieNapora = new ColorBGRA(254, 192, 118, 255);
            TorrOlgosso = new ColorBGRA(254, 192, 118, 255);
            ArmiosBell = new ColorBGRA(254, 192, 118, 255);
            ZacharieDesmarais = new ColorBGRA(254, 192, 118, 255);
            MinaraAnenima = new ColorBGRA(254, 192, 118, 255);
            IgnaPhoenix = new ColorBGRA(254, 192, 118, 255);
            JonahUnchained = new ColorBGRA(254, 192, 118, 255);
            DamoiTui = new ColorBGRA(254, 192, 118, 255);
            XandroBlooddrinker = new ColorBGRA(254, 192, 118, 255);
            VickasGiantbone = new ColorBGRA(254, 192, 118, 255);
            EoinGreyfur = new ColorBGRA(254, 192, 118, 255);
            TinevinHighdove = new ColorBGRA(254, 192, 118, 255);
            MagnusStonethorn = new ColorBGRA(254, 192, 118, 255);
            IonDarkshroud = new ColorBGRA(254, 192, 118, 255);
            AshLessard = new ColorBGRA(254, 192, 118, 255);
            WilorinDemontamer = new ColorBGRA(254, 192, 118, 255);
            AugustinaSolaria = new ColorBGRA(254, 192, 118, 255);
            DenaLorenni = new ColorBGRA(254, 192, 118, 255);
            VanthAgiel = new ColorBGRA(254, 192, 118, 255);
            LaelFuria = new ColorBGRA(254, 192, 118, 255);

            TormentedArsonist = new ColorBGRA(198, 255, 198, 255);
            TormentedFreezer = new ColorBGRA(198, 255, 198, 255);
            TormentedBlasphemer = new ColorBGRA(198, 255, 198, 255);
            TormentedCannibal = new ColorBGRA(198, 255, 198, 255);
            TormentedCharlatan = new ColorBGRA(198, 255, 198, 255);
            TormentedCutthroat = new ColorBGRA(198, 255, 198, 255);
            TormentedEmbezzler = new ColorBGRA(255, 0, 255, 255);
            TormentedCounterfeiter = new ColorBGRA(198, 255, 198, 255);
            TormentedMartyr = new ColorBGRA(198, 255, 198, 255);
            TormentedMutilator = new ColorBGRA(198, 255, 198, 255);
            TormentedNecromancer = new ColorBGRA(198, 255, 198, 255);
            TormentedPoisoner = new ColorBGRA(198, 255, 198, 255);
            TormentedRogue = new ColorBGRA(198, 255, 198, 255);
            TormentedLibrarian = new ColorBGRA(255, 255, 0, 255);
            TormentedSmuggler = new ColorBGRA(175, 96, 37, 255);
            TormentedSpy = new ColorBGRA(198, 255, 198, 255);
            TormentedExperimenter = new ColorBGRA(198, 255, 198, 255);
            TormentedThief = new ColorBGRA(198, 255, 198, 255);
            TormentedThug = new ColorBGRA(198, 255, 198, 255);
            TormentedCorrupter = new ColorBGRA(198, 255, 198, 255);
            TormentedWarlord = new ColorBGRA(198, 255, 198, 255);
            TormentedFisherman = new ColorBGRA(198, 255, 198, 255);

            TextPositionX = new RangeNode<int>(50, 0, 100);
            TextPositionY = new RangeNode<int>(85, 0, 100);
        }
        public RangeNode<int> TextPositionX { get; set; }
        public RangeNode<int> TextPositionY { get; set; }
        public ToggleNode Masters { get; set; }
        public ToggleNode Exiles { get; set; }
        public ToggleNode Tormented { get; set; }
        public ToggleNode Labyrinth { get; set; }
        public ToggleNode Strongboxes { get; set; }
        public RangeNode<int> TextSize { get; set; }
        public ColorNode BackgroundColor { get; set; }
        public ColorNode DefaultTextColor { get; set; }
        public ColorNode AreaTextColor { get; set; }
        public ColorNode HasCorruptedArea { get; set; }
        public ColorNode MasterZana { get; set; }
        public ColorNode MasterCatarina { get; set; }
        public ColorNode MasterTora { get; set; }
        public ColorNode MasterVorici { get; set; }
        public ColorNode MasterHaku { get; set; }
        public ColorNode MasterElreon { get; set; }
        public ColorNode MasterVagan { get; set; }
        public ColorNode MasterKrillson { get; set; }

        public ColorNode DivinersStrongbox { get; set; }
        public ColorNode ArcanistStrongbox { get; set; }
        public ColorNode ArtisanStrongbox { get; set; }
        public ColorNode CartographerStrongbox { get; set; }
        public ColorNode GemcutterStrongbox { get; set; }
        public ColorNode JewellerStrongbox { get; set; }
        public ColorNode BlacksmithStrongbox { get; set; }
        public ColorNode ArmourerStrongbox { get; set; }
        public ColorNode OrnateStrongbox { get; set; }
        public ColorNode LargeStrongbox { get; set; }
        public ColorNode PerandusStrongbox { get; set; }
        public ColorNode KaomStrongbox { get; set; }
        public ColorNode MalachaiStrongbox { get; set; }
        public ColorNode EpicStrongbox { get; set; }
        public ColorNode SimpleStrongbox { get; set; }
        public ColorNode OrraGreengate { get; set; }
        public ColorNode ThenaMoga { get; set; }
        public ColorNode AntalieNapora { get; set; }
        public ColorNode TorrOlgosso { get; set; }
        public ColorNode ArmiosBell { get; set; }
        public ColorNode ZacharieDesmarais { get; set; }
        public ColorNode MinaraAnenima { get; set; }
        public ColorNode IgnaPhoenix { get; set; }
        public ColorNode JonahUnchained { get; set; }
        public ColorNode DamoiTui { get; set; }
        public ColorNode XandroBlooddrinker { get; set; }
        public ColorNode VickasGiantbone { get; set; }
        public ColorNode EoinGreyfur { get; set; }
        public ColorNode TinevinHighdove { get; set; }
        public ColorNode MagnusStonethorn { get; set; }
        public ColorNode IonDarkshroud { get; set; }
        public ColorNode AshLessard { get; set; }
        public ColorNode WilorinDemontamer { get; set; }
        public ColorNode AugustinaSolaria { get; set; }
        public ColorNode DenaLorenni { get; set; }
        public ColorNode VanthAgiel { get; set; }
        public ColorNode LaelFuria { get; set; }


        public ColorNode TormentedArsonist { get; set; }
        public ColorNode TormentedFreezer { get; set; }
        public ColorNode TormentedBlasphemer { get; set; }
        public ColorNode TormentedCannibal { get; set; }
        public ColorNode TormentedCharlatan { get; set; }
        public ColorNode TormentedCutthroat { get; set; }
        public ColorNode TormentedEmbezzler { get; set; }
        public ColorNode TormentedCounterfeiter { get; set; }
        public ColorNode TormentedMartyr { get; set; }
        public ColorNode TormentedMutilator { get; set; }
        public ColorNode TormentedNecromancer { get; set; }
        public ColorNode TormentedPoisoner { get; set; }
        public ColorNode TormentedRogue { get; set; }
        public ColorNode TormentedLibrarian { get; set; }
        public ColorNode TormentedSmuggler { get; set; }
        public ColorNode TormentedSpy { get; set; }
        public ColorNode TormentedExperimenter { get; set; }
        public ColorNode TormentedThief { get; set; }
        public ColorNode TormentedThug { get; set; }
        public ColorNode TormentedCorrupter { get; set; }
        public ColorNode TormentedWarlord { get; set; }
        public ColorNode TormentedFisherman { get; set; }

    }
}