using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.World;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class Memory : Entity {
    public float CollisionRadius { get; }

    public int memoryIndex { get; private set; }

    private const float MEMORY_RADIUS = 0.2f;
    private static Color Color { get; } = new Color(0, 128, 160, 255);

    public Memory(GameWorld world, Vector2 position, int memoryIndex)
        : base(world, "Memory", position) {

        CollisionRadius = 0.2f;
        this.memoryIndex = memoryIndex;
    }

    public override void Render(float dT) {
        Raylib.DrawCircleV(Position * GameWorld.TILE_SIZE, MEMORY_RADIUS * GameWorld.TILE_SIZE, Color);

        if (Application.DRAW_DEBUG)
            Raylib.DrawCircleLines((int)(Position.X * GameWorld.TILE_SIZE), (int)(Position.Y * GameWorld.TILE_SIZE), CollisionRadius * GameWorld.TILE_SIZE, Raylib.LIME);
    }

    public override void Update(float dT) {
        float minDistance = World.Player.CollisionRadius + CollisionRadius;            
        if ((World.Player.Position - Position).LengthSquared() <= minDistance * minDistance){
            MemoryTracker.CollectMemory(memoryIndex);
            World.Memory = null;
        }
    }
}
