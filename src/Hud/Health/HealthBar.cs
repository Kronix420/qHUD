namespace qHUD.Hud.Health
{
    using Models;
    using Models.Enums;
    using Poe.Components;
    public class HealthBar
    {
        private readonly bool isHostile;

        public HealthBar(EntityWrapper entity, HealthBarSettings settings)
        {
            Entity = entity;
            if (entity.HasComponent<Player>())
            {
                Type = CreatureType.Player;
                Settings = settings.Players;
                IsValid = true;
            }
            else if (entity.HasComponent<Monster>())
            {
                IsValid = true;
                if (entity.IsHostile)
                {
                    isHostile = true;
                    switch (entity.GetComponent<ObjectMagicProperties>().Rarity)
                    {
                        case MonsterRarity.White:
                            Type = CreatureType.Normal;
                            Settings = settings.NormalEnemy;
                            break;

                        case MonsterRarity.Magic:
                            Type = CreatureType.Magic;
                            Settings = settings.MagicEnemy;
                            break;

                        case MonsterRarity.Rare:
                            Settings = settings.RareEnemy;
                            Type = CreatureType.Rare;
                            break;

                        case MonsterRarity.Unique:
                            Settings = settings.UniqueEnemy;
                            Type = CreatureType.Unique;
                            break;
                    }
                }
                else
                {
                    Type = CreatureType.Minion;
                    Settings = settings.Minions;
                }
            }
            Life = Entity.GetComponent<Life>();
        }

        public Life Life { get; }
        public EntityWrapper Entity { get; }
        public bool IsValid { get; private set; }
        public UnitSettings Settings { get; }
        public CreatureType Type { get; private set; }

        public bool IsShow(bool showEnemy)
        {
            return !isHostile ? Settings.Enable.Value : Settings.Enable && showEnemy && isHostile;
        }
    }
}