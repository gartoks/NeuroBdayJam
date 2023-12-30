using NeuroBdayJam.Game.World;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal abstract class Entity {
    public GameWorld? World { get; private set; }

    public Guid Id { get; }
    public string Name { get; }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }

    public bool IsDead { get; protected set; }

    protected Entity(string name, Vector2 position) {
        Id = Guid.NewGuid();
        Name = name;
        Position = position;
        Rotation = 0;
    }

    public void Load(GameWorld world) {
        World = world;
        LoadInternal();
    }

    public virtual void LoadInternal() {
    }

    public void Unload() {
        IsDead = true;
        UnloadInternal();
        World = null;
    }

    public virtual void UnloadInternal() {
    }

    public virtual void Update(float dT) {
    }

    public virtual void Render(float dT) {
    }
}
