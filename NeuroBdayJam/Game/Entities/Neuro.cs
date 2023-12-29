using NeuroBdayJam.App;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class Neuro : Entity {
    private TextureResource IdleTexture { get; }

    public float CollisionRadius { get; }

    public float Speed { get; private set; }

    public Neuro(GameWorld world, Vector2 position)
        : base(world, "Neuro", position) {

        IdleTexture = ResourceManager.TextureLoader.Get("player");

        Speed = 2.5f;
        CollisionRadius = 0.3f;
    }

    public override void Render(float dT) {
        IdleTexture.Draw(new Rectangle(Position.X * GameWorld.TILE_SIZE, (Position.Y - 0.5f) * GameWorld.TILE_SIZE, GameWorld.TILE_SIZE, GameWorld.TILE_SIZE), new Vector2(0.5f, 0.5f), 0, new Color(80, 92, 160, 255));

        if (Application.DRAW_DEBUG)
            Raylib.DrawCircleLines((int)(Position.X * GameWorld.TILE_SIZE), (int)(Position.Y * GameWorld.TILE_SIZE), CollisionRadius * GameWorld.TILE_SIZE, Raylib.LIME);
    }

    public override void Update(float dT) {
        float speed = Speed;
        if (Input.IsHotkeyDown(GameHotkeys.SPRINT))
            speed *= 1.5f;
        if (Input.IsHotkeyDown(GameHotkeys.SNEAK))
            speed *= 0.5f;

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

        movement = Vector2.Normalize(movement) * speed;
        Vector2 newPosition = Position + movement * dT;

        IReadOnlyList<Rectangle> colliders = World.GetSurroundingTileColliders(newPosition);

        Vector2 mtv = Collisions.ResolveCollisionCircleRects(newPosition, CollisionRadius, colliders);
        Position = newPosition + mtv;
    }
}
