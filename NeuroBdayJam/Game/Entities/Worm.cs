using NeuroBdayJam.Game.World;
using NeuroBdayJam.Util;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal class Worm : Entity {
    private const int SEGMENT_COUNT = 3;
    private const float MAX_SEGMENT_DISTANCE = 0.3f;
    private const float SEGMENT_RADIUS = 0.2f;
    private static Color Color { get; } = new Color(16, 128, 16, 255);

    public float Speed { get; }

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
        Vector2 vectorToPlayer = World.Player.Position - Position;

        if (vectorToPlayer.LengthSquared() < 0.05f)
            return;

        Vector2 directionToPlayer = Vector2.Normalize(vectorToPlayer);

        Vector2 newPosition = Position + directionToPlayer * Speed * dT;
        Vector2 mtv = Collisions.ResolveCollisionCircleRects(newPosition, SEGMENT_RADIUS, World.GetSurroundingTileColliders(newPosition));
        Position = newPosition + mtv;
        Vector2 last = Position;
        for (int i = 0; i < SEGMENT_COUNT; i++) {
            Vector2 segmentPosition = SegmentPositions[i];

            Vector2 vectorToPrevSegment = segmentPosition - last;

            if (vectorToPrevSegment.LengthSquared() <= MAX_SEGMENT_DISTANCE * MAX_SEGMENT_DISTANCE)
                continue;

            newPosition = last + Vector2.Normalize(vectorToPrevSegment) * MAX_SEGMENT_DISTANCE;
            mtv = Collisions.ResolveCollisionCircleRects(newPosition, SEGMENT_RADIUS, World.GetSurroundingTileColliders(newPosition));
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
