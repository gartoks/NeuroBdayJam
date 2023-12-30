using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.World;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Memories;
internal sealed class Memory : Entity {
    private const float MEMORY_RADIUS = 0.2f;
    private static Color Color { get; } = new Color(0, 128, 160, 255);

    public override float CollisionRadius => MEMORY_RADIUS;
    public int MemoryIndex { get; private set; }

    public override Vector2 Facing => Vector2.Zero;

    public Memory(Vector2 position, int memoryIndex)
        : base("Memory", position) {

        MemoryIndex = memoryIndex;
    }

    public override void Render(float dT) {
        Raylib.DrawCircleV(Position * GameWorld.TILE_SIZE, MEMORY_RADIUS * GameWorld.TILE_SIZE, Color);

        if (Application.DRAW_DEBUG)
            Raylib.DrawCircleLines((int)(Position.X * GameWorld.TILE_SIZE), (int)(Position.Y * GameWorld.TILE_SIZE), CollisionRadius * GameWorld.TILE_SIZE, Raylib.LIME);
    }

    public override void Update(float dT) {
        float minDistance = World!.Player.CollisionRadius + CollisionRadius;
        if ((World.Player.Position - Position).LengthSquared() > minDistance * minDistance)
            return;

        MemoryTracker.CollectMemory(MemoryIndex);

        World.ActiveMemory = null;
        IsDead = true;
    }
}
