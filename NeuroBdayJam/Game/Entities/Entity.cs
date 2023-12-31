using NeuroBdayJam.Game.World;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal abstract class Entity {
    public GameWorld? World { get; private set; }

    public Guid Id { get; }
    public string Name { get; }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }

    public int ZIndex { get; protected set; }
    public abstract float CollisionRadius { get; }
    public abstract Vector2 Facing { get; }

    public eEntityStates State { get; protected set; }

    public bool IsDead { get; protected set; }
    public bool HasMoved { get; protected set; }

    protected Entity(string name, Vector2 position) {
        Id = Guid.NewGuid();
        Name = name;
        Position = position;
        Rotation = 0;
        ZIndex = 0;

        HasMoved = false;
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

    public void SetState(eEntityStates state) {
        State = State | state;
    }

    public void RemoveState(eEntityStates state) {
        State = State & (~state);
    }

    public virtual void UnloadInternal() {
    }

    public virtual void Update(float dT) {
    }

    public virtual void Render(float dT) {
    }
}
