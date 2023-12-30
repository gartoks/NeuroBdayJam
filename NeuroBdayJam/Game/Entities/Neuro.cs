using NeuroBdayJam.App;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Graphics;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class Neuro : Entity {
    private const float NOISE_STEP = 0.1f;

    private TextureAtlas AnimationsTextureAtlas { get; set; }
    private FrameAnimator FaceUpAnimator { get; set; }
    private FrameAnimator FaceDownAnimator { get; set; }
    private FrameAnimator FaceLeftAnimator { get; set; }
    private FrameAnimator FaceRightAnimator { get; set; }

    public float CollisionRadius { get; }
    public float Speed { get; private set; }

    private Vector2 Facing => Raylib.GetMousePosition() / GameWorld.TILE_SIZE - Position;

    private bool SpawnedStep { get; set; }
    private (int x, int y) MovementDirection { get; set; }
    private FrameAnimator LastAnimator { get; set; }
    private bool WasMoving { get; set; }

    public Neuro(Vector2 position)
        : base("Neuro", position) {

        Speed = 2.0f;
        CollisionRadius = 0.3f;

        MovementDirection = (0, 0);
        LastAnimator = null;
        WasMoving = false;
    }

    public override void LoadInternal() {
        AnimationsTextureAtlas = ResourceManager.TextureAtlasLoader.Get("player_animations").Resource;

        FaceUpAnimator = new FrameAnimator(1f / 12f);
        LoadFrameAnimator(FaceUpAnimator, "up");

        FaceDownAnimator = new FrameAnimator(1f / 12f);
        LoadFrameAnimator(FaceDownAnimator, "down");

        FaceLeftAnimator = new FrameAnimator(1f / 12f);
        LoadFrameAnimator(FaceLeftAnimator, "left");

        FaceRightAnimator = new FrameAnimator(1f / 12f);
        LoadFrameAnimator(FaceRightAnimator, "right");

        LastAnimator = FaceDownAnimator;
        LastAnimator.StartSequence("idle");
    }

    public override void Render(float dT) {
        //IdleTexture.Draw(new Rectangle(Position.X * GameWorld.TILE_SIZE, (Position.Y - 0.3f) * GameWorld.TILE_SIZE, GameWorld.TILE_SIZE, GameWorld.TILE_SIZE), new Vector2(0.5f, 0.5f), 0, new Color(80, 92, 160, 255));

        FrameAnimator animator = GetAnimator(out bool isMoving);

        if (animator.IsReady) {
            if (isMoving)
                animator.StartSequence("walk");
            else
                animator.StartSequence("idle");
        }

        if (LastAnimator != animator) {
            if (isMoving && WasMoving) {
                animator.StartSequence("walk", LastAnimator.FrameIndex);
            } else if (isMoving)
                animator.StartSequence("walk");
            else
                animator.StartSequence("idle");

            LastAnimator = animator;
            WasMoving = isMoving;
        }

        animator.Render(dT, new Rectangle(Position.X * GameWorld.TILE_SIZE, (Position.Y - 0.3f) * GameWorld.TILE_SIZE, GameWorld.TILE_SIZE, GameWorld.TILE_SIZE), 0, new Vector2(0.5f, 0.5f), Raylib.WHITE/*new Color(80, 92, 160, 255)*/);

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

        int xDir = 0;
        int yDir = 0;
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_UP)) {
            movement.Y -= Speed;
            yDir--;
        }
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_DOWN)) {
            movement.Y += Speed;
            yDir++;
        }
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_LEFT)) {
            movement.X -= Speed;
            xDir--;
        }
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_RIGHT)) {
            movement.X += Speed;
            xDir++;
        }
        MovementDirection = (xDir, yDir);

        if (movement.LengthSquared() <= 0)
            return;

        movement = Vector2.Normalize(movement) * speed;
        Vector2 newPosition = Position + movement * dT;

        IReadOnlyList<Rectangle> colliders = World.GetSurroundingTileColliders(newPosition);

        Vector2 mtv = Collisions.ResolveCollisionCircleRects(newPosition, CollisionRadius, colliders);
        newPosition += mtv;

        int currentTileX = (int)Position.X;
        int currentTileY = (int)Position.Y;
        int newTileX = (int)newPosition.X;
        int newTileY = (int)newPosition.Y;

        if (currentTileX != newTileX || currentTileY != newTileY)
            SpawnedStep = false;

        if (mtv.LengthSquared() == 0) {
            //AudioManager.PlaySound("player_step");

            if (!SpawnedStep) {
                World.AddEntity(new NoiseSpawner(newPosition, NOISE_STEP * speed / Speed));
                SpawnedStep = true;
            }
        }

        Position = newPosition;
    }

    private FrameAnimator GetAnimator(out bool isMoving) {
        if (MovementDirection.x == 0 && MovementDirection.y == 0) {
            isMoving = false;

            Vector2 facing = Vector2.Normalize(Facing);
            float xAxis = Vector2.Dot(facing, Vector2.UnitX);
            float yAxis = Vector2.Dot(facing, Vector2.UnitY);

            if (Math.Abs(xAxis) > Math.Abs(yAxis)) {
                if (xAxis > 0)
                    return FaceRightAnimator;
                else
                    return FaceLeftAnimator;
            } else {
                if (yAxis > 0)
                    return FaceDownAnimator;
                else
                    return FaceUpAnimator;
            }
        } else {
            isMoving = true;

            if (MovementDirection.y > 0)
                return FaceDownAnimator;
            else if (MovementDirection.y < 0)
                return FaceUpAnimator;
            else if (MovementDirection.x > 0)
                return FaceRightAnimator;
            else if (MovementDirection.x < 0)
                return FaceLeftAnimator;
            else
                return FaceDownAnimator;
        }
    }

    private void LoadFrameAnimator(FrameAnimator animator, string direction) {
        for (int i = 0; i < 4; i++)
            animator.AddFrameKey($"idle_{i}", AnimationsTextureAtlas.GetSubTexture($"idle_{direction}_{i}")!);
        for (int i = 0; i < 8; i++)
            animator.AddFrameKey($"walk_{i}", AnimationsTextureAtlas.GetSubTexture($"walk_{direction}_{i}")!);

        animator.AddFrameSequence("idle", 1, "idle_0", "idle_0", "idle_0", "idle_0", "idle_0", "idle_1", "idle_1", "idle_1", "idle_2", "idle_2", "idle_2", "idle_2", "idle_2", "idle_3", "idle_3", "idle_3");
        animator.AddFrameSequence("walk", 1, "walk_0", "walk_0", "walk_1", "walk_1", "walk_2", "walk_2", "walk_3", "walk_3", "walk_4", "walk_4", "walk_5", "walk_5", "walk_6", "walk_6", "walk_7", "walk_7");
        animator.SetDefaultSequence("idle");
    }
}
