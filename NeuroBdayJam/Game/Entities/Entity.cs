using NeuroBdayJam.Game.World;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal abstract class Entity {
    protected GameWorld World { get; }

    public Guid Id { get; }
    public string Name { get; }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }

    protected Entity(GameWorld world, string name, Vector2 position) {
        World = world;
        Id = Guid.NewGuid();
        Name = name;
        Position = position;
        Rotation = 0;
    }

    public virtual void Update(float dT) {
    }

    public virtual void Render(float dT) {
    }
}
