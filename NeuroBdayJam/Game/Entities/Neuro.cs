using NeuroBdayJam.App;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class Neuro : Entity {
    private TextureResource IdleTexture { get; }

    public float Speed { get; private set; }

    private List<Rectangle> DEBUG_IsColliding { get; } = new List<Rectangle>();

    public Neuro(GameWorld world, Vector2 position)
        : base(world, "Neuro", position) {

        IdleTexture = ResourceManager.TextureLoader.Get("player");

        Speed = 2;
    }

    public override void Render(float dT) {
        IdleTexture.Draw(new Rectangle(Position.X * GameWorld.TILE_SIZE, (Position.Y - 0.5f) * GameWorld.TILE_SIZE, GameWorld.TILE_SIZE, GameWorld.TILE_SIZE), new Vector2(0.5f, 0.5f));

        Raylib.DrawCircleLines((int)(Position.X * GameWorld.TILE_SIZE), (int)(Position.Y * GameWorld.TILE_SIZE), 0.3f * GameWorld.TILE_SIZE, DEBUG_IsColliding.Count > 0 ? Raylib.RED : Raylib.LIME);

        foreach (Rectangle collider in DEBUG_IsColliding)
            Raylib.DrawRectangleLinesEx(collider, 1, Raylib.RED);
    }

    public override void Update(float dT) {
        Vector2 movement = Vector2.Zero;

        if (Input.IsHotkeyDown(GameHotkeys.MOVE_UP))
            movement.Y -= Speed;
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_DOWN))
            movement.Y += Speed;
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_LEFT))
            movement.X -= Speed;
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_RIGHT))
            movement.X += Speed;

        if (movement.LengthSquared() <= 0)
            return;

        movement = Vector2.Normalize(movement) * Speed;
        Vector2 newPosition = Position + movement * dT;

        IReadOnlyList<Rectangle> colliders = World.GetSurroundingTileColliders(newPosition);

        DEBUG_IsColliding.Clear();
        foreach (Rectangle collider in colliders) {
            if (Raylib.CheckCollisionCircleRec(Position * GameWorld.TILE_SIZE, 0.3f * GameWorld.TILE_SIZE, collider)) {
                DEBUG_IsColliding.Add(collider);
            }
        }

        Position = newPosition;
    }
}
