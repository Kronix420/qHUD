namespace qHUD.Models
{
    using System.Collections.Generic;
    using Controllers;
    using Interfaces;
    using Poe;
    using Poe.Components;
    using Vector3 = SharpDX.Vector3;
    public class EntityWrapper : IEntity
    {
        private readonly Dictionary<string, int> components;
        private readonly GameController gameController;
        private readonly Entity internalEntity;
        public bool IsInList = true;

        public EntityWrapper(GameController Poe, Entity entity)
        {
            gameController = Poe;
            internalEntity = entity;
            components = internalEntity.GetComponents();
            Path = internalEntity.Path;
            Id = internalEntity.Id;
            LongId = internalEntity.LongId;
        }

        public EntityWrapper(GameController Poe, int address) : this(Poe, Poe.Game.GetObject<Entity>(address)) { }
        public string Path { get; }
        public bool IsValid => internalEntity.IsValid && IsInList && Id == internalEntity.Id;
        public int Address => internalEntity.Address;
        public int Id { get; }
        public bool IsHostile => internalEntity.IsHostile;
        public long LongId { get; }
        public bool IsAlive => GetComponent<Life>().CurHP > 0;

        public Vector3 Pos
        {
            get
            {
                var p = GetComponent<Positioned>();
                return new Vector3(p.X, p.Y, GetComponent<Render>().Z);
            }
        }

        public T GetComponent<T>() where T : Component, new()
        {
            string name = typeof(T).Name;
            return gameController.Game.GetObject<T>(components.ContainsKey(name) ? components[name] : 0);
        }

        public bool HasComponent<T>() where T : Component, new()
        {
            return components.ContainsKey(typeof(T).Name);
        }

        public override bool Equals(object obj)
        {
            var entity = obj as EntityWrapper;
            return entity != null && entity.LongId == LongId;
        }

        public override int GetHashCode()
        {
            return LongId.GetHashCode();
        }

        public override string ToString()
        {
            return "EntityWrapper: " + Path;
        }
    }
}