using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Graphics;
using NeuroBdayJam.Graphics.Gradients;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Memories;
internal sealed class Memory : Entity {
    private const float MEMORY_RADIUS = 0.2f;
    private static MemoryGradient ColorGradient { get; } = new MemoryGradient();

    public override float CollisionRadius => MEMORY_RADIUS;
    public int MemoryIndex { get; private set; }

    private IReadOnlyList<MemoryBlock> MemoryBlocks { get; }
    private float Time { get; set; }

    public override Vector2 Facing => Vector2.Zero;

    public Memory(Vector2 position, int memoryIndex)
        : base("Memory", position) {

        MemoryIndex = memoryIndex;
        Time = 0;
    }
    public Memory(Vector2 position)
        : base("Memory", position) {

        MemoryIndex = 0;
        Time = 0;

        List<MemoryBlock> memoryBlocks = new List<MemoryBlock>();
        for (int i = 0; i < 7; i++) {
            memoryBlocks.Add(new MemoryBlock(
                Random.Shared.NextVector(-0.15f, 0.15f),
                ColorSpeed: Random.Shared.NextSingle(0.5f, 1.5f),
                RotationSpeed: Random.Shared.NextSingle(-0.5f, 0.5f),
                ScaleSpeed: Random.Shared.NextSingle(0.01f, 0.3f)));
        }
        MemoryBlocks = memoryBlocks;
    }

    public override void LoadInternal(){
        base.LoadInternal();

        MemoryIndex = World.MemoryTracker.GetNextUncollectedMemory();
    }

    public override void UnloadInternal(){
        base.UnloadInternal();

        World.ActiveMemory = null;
    }

    public override void Render(float dT) {
        Time += dT;

        foreach (MemoryBlock memBlock in MemoryBlocks) {
            float tColor = (Time * memBlock.ColorSpeed) % 1f;
            float tRotation = Time * memBlock.RotationSpeed;
            float tScale = AnimWrap(0.5f * Time * memBlock.ScaleSpeed);
            float tAlpha = AnimWrap(Time * memBlock.ScaleSpeed) * 0.5f;

            Color color = ColorGradient.GetColor(tColor).ChangeAlpha((int)(255 * tAlpha));
            float rotation = tRotation * 360f;
            float scale = 4 * tScale;

            float x = (Position.X + memBlock.Offset.X + -MEMORY_RADIUS / 2f * scale) * GameWorld.TILE_SIZE;
            float y = (Position.Y + memBlock.Offset.Y + -MEMORY_RADIUS / 2f * scale) * GameWorld.TILE_SIZE;
            float w = MEMORY_RADIUS * GameWorld.TILE_SIZE * scale;
            float h = MEMORY_RADIUS * GameWorld.TILE_SIZE * scale;

            Raylib.DrawRectanglePro(new Rectangle(x, y, w, h), new Vector2(w / 2f, h / 2f), rotation, color);
        }

        if (Application.DRAW_DEBUG) {
            Raylib.DrawCircleLines((int)(Position.X * GameWorld.TILE_SIZE), (int)(Position.Y * GameWorld.TILE_SIZE), CollisionRadius * GameWorld.TILE_SIZE, Raylib.LIME);
            Raylib.DrawTextEx(Renderer.GuiFont.Resource, $"{MemoryIndex}", (Position + new Vector2(0, 0.35f)) * GameWorld.TILE_SIZE, 30, 0, Raylib.WHITE);
        }
    }

    public override void Update(float dT) {
        float minDistance = World!.Player.CollisionRadius + CollisionRadius;
        if ((World.Player.Position - Position).LengthSquared() > minDistance * minDistance)
            return;

        World.MemoryTracker.CollectMemory(MemoryIndex);

        World.ActiveMemory = null;
        IsDead = true;
    }

    private static float AnimWrap(float t) => (MathF.Cos(t * MathF.Tau + MathF.PI) / 2f + 0.5f) % 1f;

    private record MemoryBlock(Vector2 Offset, float ColorSpeed, float RotationSpeed, float ScaleSpeed);
}
