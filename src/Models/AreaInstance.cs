namespace qHUD.Models
{
    using Poe.RemoteMemoryObjects;
    public sealed class AreaInstance
    {
        public int RealLevel { get; }
        public int NominalLevel { get; }
        public string Name { get; }
        public int Act { get; }
        public bool IsTown { get; }
        public bool IsHideout { get; }
        public bool HasWaypoint { get; }
        public int Hash { get; }

        public AreaInstance(AreaTemplate area, int hash, int realLevel)
        {
            Hash = hash;
            RealLevel = realLevel;
            NominalLevel = area.NominalLevel;
            Name = area.Name;
            Act = area.Act;
            IsTown = area.IsTown;
            HasWaypoint = area.HasWaypoint;
            IsHideout = Name.Contains("Hideout");
        }

        public override string ToString() { return $"{Name} ({RealLevel}) #{Hash}"; }
        public string DisplayName => string.Concat(Name, " (", RealLevel, ")");

    }
}