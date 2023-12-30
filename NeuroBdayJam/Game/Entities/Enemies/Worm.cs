using NeuroBdayJam.Game.World;
using NeuroBdayJam.Util;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Enemies;
internal class Worm : Enemy {
    private const int SEGMENT_COUNT = 3;
    private const float MAX_SEGMENT_DISTANCE = 0.3f;
    private const float SEGMENT_RADIUS = 0.2f;
    private static Color Color { get; } = new Color(16, 128, 16, 255);

    public override float CollisionRadius => SEGMENT_RADIUS;
    public float Speed { get; }

    private Vector2 _Facing { get; set; }
    public override Vector2 Facing => _Facing;

    public Vector2[] SegmentPositions { get; }

    public Worm(Vector2 position)
        : base("Worm", position) {

        Speed = 1f;

        SegmentPositions = new Vector2[SEGMENT_COUNT];

        Vector2 last = position;
        for (int i = 0; i < SEGMENT_COUNT; i++) {
            SegmentPositions[i] = last + Random.Shared.NextRandomInCircleUniformly(MAX_SEGMENT_DISTANCE);
            last = SegmentPositions[i];
        }
    }

    public override void LoadInternal() {
        base.LoadInternal();
    }

    public override void Update(float dT) {
        if (World!.Player.State.HasFlag(eEntityStates.Hidden))
            return;

        if (State.HasFlag(eEntityStates.Stunned))
            return;

        Vector2 vectorToPlayer = World.Player.Position - Position;

        Vector2 directionToPlayer = Vector2.Normalize(vectorToPlayer);
        _Facing = directionToPlayer;

        Vector2 movement = directionToPlayer * Speed;

        HasMoved = movement.LengthSquared() > 0;

        if (vectorToPlayer.LengthSquared() < 0.05f)
            return;

        Vector2 newPosition = Position + movement * dT;
        Vector2 mtv = Collisions.ResolveCollisionCircleRects(newPosition, CollisionRadius, World.GetSurroundingTileColliders(newPosition));
        Position = newPosition + mtv;
        Vector2 last = Position;
        for (int i = 0; i < SEGMENT_COUNT; i++) {
            Vector2 segmentPosition = SegmentPositions[i];

            Vector2 vectorToPrevSegment = segmentPosition - last;

            if (vectorToPrevSegment.LengthSquared() <= MAX_SEGMENT_DISTANCE * MAX_SEGMENT_DISTANCE)
                continue;

            newPosition = last + Vector2.Normalize(vectorToPrevSegment) * MAX_SEGMENT_DISTANCE;
            mtv = Collisions.ResolveCollisionCircleRects(newPosition, CollisionRadius, World.GetSurroundingTileColliders(newPosition));
            SegmentPositions[i] = newPosition + mtv;

            last = SegmentPositions[i];
        }
    }

    public override void Render(float dT) {
        Raylib.DrawCircleV(Position * GameWorld.TILE_SIZE, SEGMENT_RADIUS * GameWorld.TILE_SIZE, Color);

        Vector2 last = Position;
        for (int i = 0; i < SEGMENT_COUNT; i++) {
            Vector2 segmentPosition = SegmentPositions[i];
            Raylib.DrawCircleV(segmentPosition * GameWorld.TILE_SIZE, SEGMENT_RADIUS * GameWorld.TILE_SIZE, Color);
            //Raylib.DrawLineEx(last * GameWorld.TILE_SIZE, segmentPosition * GameWorld.TILE_SIZE, 0.1f * GameWorld.TILE_SIZE, Raylib.RED);
            last = segmentPosition;
        }
    }
}
