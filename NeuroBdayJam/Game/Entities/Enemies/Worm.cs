using NeuroBdayJam.Game.World;
using NeuroBdayJam.Graphics;
using NeuroBdayJam.Util;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Enemies;
internal class Worm : Enemy {
    private const int SEGMENT_COUNT = 8;
    private const int LEG_COUNT = 2;
    private const float MAX_SEGMENT_DISTANCE = 0.6f;
    private const float MAX_LEG_DISTANCE = 0.01f;
    private const float SEGMENT_SIZE = 0.8f;
    private static Color IdleColor { get; } = new Color(16, 64, 16, 255);
    private static Color SearchColor { get; } = new Color(128, 128, 16, 255);
    private static Color ChaseColor { get; } = new Color(255, 16, 16, 255);

    public override float CollisionRadius => SEGMENT_SIZE / 2f;
    public float Speed { get; }
    public int ThreatLevel { get; private set; }

    private Vector2 _Facing { get; set; }
    public override Vector2 Facing => _Facing;

    public WormSegment[] Segments { get; }

    public Worm(Vector2 position)
        : base("Worm", position) {

        Speed = 1f;

        Segments = new WormSegment[SEGMENT_COUNT];

        Segments[0] = new WormSegment(Position);
        WormSegment last = Segments[0];
        for (int i = 1; i < SEGMENT_COUNT; i++) {
            Segments[i] = new WormSegment(last.Position + Random.Shared.NextRandomInCircleUniformly(MAX_SEGMENT_DISTANCE));
            last = Segments[i];
        }
    }

    public override void LoadInternal() {
        base.LoadInternal();
    }

    public override void Update(float dT) {
        ThreatLevel = 0;

        if (World!.Player.State.HasFlag(eEntityStates.Hidden))
            return;

        if (State.HasFlag(eEntityStates.Stunned))
            return;

        Vector2 vectorToPlayer = World.Player.Position - Position;

        WorldTile? currenTile = World?.GetTile(World.WorldToTileIndexSpace(Position));

        if (vectorToPlayer.LengthSquared() > 10 * 10 || (World!.Player.Position - World.PlayerSpawn).LengthSquared() < 10 * 10) {
            vectorToPlayer = Vector2.Zero;
        } else if (vectorToPlayer.LengthSquared() < 3 * 3 || (currenTile != null && currenTile?.NoiseValue > 0)) {
            ThreatLevel = 1;
        }

        Vector2 directionToPlayer = Vector2.Normalize(vectorToPlayer);
        _Facing = directionToPlayer;

        Vector2 movement = directionToPlayer * (Speed + ThreatLevel * Speed * 1.1f);

        HasMoved = movement.LengthSquared() > 0;

        if (vectorToPlayer.LengthSquared() < 0.05f)
            return;

        Vector2 newPosition = Position + movement * dT;
        Vector2 mtv = Collisions.ResolveCollisionCircleRects(newPosition, CollisionRadius, World.GetSurroundingTileColliders(newPosition));
        Position = newPosition + mtv;
        Segments[0].Position = Position;
        WormSegment last = Segments[0];
        for (int i = 1; i < SEGMENT_COUNT; i++) {
            WormSegment segment = Segments[i];

            Vector2 vectorToPrevSegment = segment.Position - last.Position;

            if (vectorToPrevSegment.LengthSquared() <= MAX_SEGMENT_DISTANCE * MAX_SEGMENT_DISTANCE)
                continue;

            newPosition = last.Position + Vector2.Normalize(vectorToPrevSegment) * MAX_SEGMENT_DISTANCE;
            mtv = Collisions.ResolveCollisionCircleRects(newPosition, CollisionRadius, World.GetSurroundingTileColliders(newPosition));
            Segments[i].Position = newPosition + mtv;

            last = Segments[i];
        }

        for (int i = 0; i < SEGMENT_COUNT; i++) {
            WormSegment segment = Segments[i];

            Vector2 facing = i == 0 ? Facing : Vector2.Normalize(segment.Position - Segments[i - 1].Position);
            facing.GetNormals(out Vector2 normal0, out Vector2 normal1);

            Vector2 lastLegPos = segment.Position;
            for (int j = 0; j < LEG_COUNT; j++) {
                Vector2 legPos = segment.LeftLegs[j];

                Vector2 legVector = legPos - lastLegPos;

                if (legVector.LengthSquared() <= MAX_LEG_DISTANCE * MAX_LEG_DISTANCE)
                    continue;

                legPos = (lastLegPos + Vector2.Normalize(legVector + normal0) * Random.Shared.NextSingle(MAX_LEG_DISTANCE * 0.1f)).RotateAround(lastLegPos, Random.Shared.NextSingle(-MathF.PI / 4f, MathF.PI / 4f));
                if (j == 0) {
                    legPos = (lastLegPos + Vector2.Normalize(normal0) * Random.Shared.NextSingle(MAX_LEG_DISTANCE * 0.1f)).RotateAround(lastLegPos, Random.Shared.NextSingle(-MathF.PI / 4f, MathF.PI / 4f));
                }

                segment.LeftLegs[j] = legPos;

                lastLegPos = legPos;
            }

            lastLegPos = segment.Position;
            for (int j = 0; j < LEG_COUNT; j++) {
                Vector2 legPos = segment.RightLegs[j];

                Vector2 legVector = legPos - lastLegPos;

                if (legVector.LengthSquared() <= MAX_LEG_DISTANCE * MAX_LEG_DISTANCE)
                    continue;

                legPos = (lastLegPos + Vector2.Normalize(legVector + normal1) * Random.Shared.NextSingle(MAX_LEG_DISTANCE * 0.1f)).RotateAround(lastLegPos, Random.Shared.NextSingle(-MathF.PI / 4f, MathF.PI / 4f));
                if (j == 0) {
                    legPos = (lastLegPos + Vector2.Normalize(normal1) * Random.Shared.NextSingle(MAX_LEG_DISTANCE * 0.1f)).RotateAround(lastLegPos, Random.Shared.NextSingle(-MathF.PI / 4f, MathF.PI / 4f));
                }

                segment.RightLegs[j] = legPos;

                lastLegPos = legPos;
            }
        }
    }

    public override void Render(float dT) {
        for (int i = 0; i < SEGMENT_COUNT; i++) {
            WormSegment segment = Segments[i];

            float rotation = segment.Rotation + Renderer.Time * 3.3f * (ThreatLevel + 1);
            float scale0 = 0.5f + (0.3f + 0.1f * ThreatLevel) * (MathF.Sin(Renderer.Time * 7 * (ThreatLevel + 1)) / 2f + 0.5f);
            float scale1 = 0.5f + (0.3f + 0.1f * ThreatLevel) * (MathF.Sin(Renderer.Time * 7 * (ThreatLevel + 1) + MathF.PI / 2f) / 2f + 0.5f);
            Color color = ThreatLevel switch {
                0 => IdleColor,
                1 => SearchColor,
                2 => ChaseColor,
                _ => IdleColor,
            };

            RaylibUtil.DrawRectLines((segment.Position) * GameWorld.TILE_SIZE, new Vector2(SEGMENT_SIZE) * scale0 * GameWorld.TILE_SIZE, -rotation, 4, color);
            RaylibUtil.DrawRectLines((segment.Position) * GameWorld.TILE_SIZE, new Vector2(SEGMENT_SIZE) * scale1 * GameWorld.TILE_SIZE, rotation + MathF.PI / 4f, 4, color);

            Vector2 lastLegPos = segment.Position;
            for (int j = 0; j < LEG_COUNT; j++) {
                Raylib.DrawLineEx(lastLegPos * GameWorld.TILE_SIZE, segment.RightLegs[j] * GameWorld.TILE_SIZE, 4, color);
                lastLegPos = segment.RightLegs[j];
            }

            lastLegPos = segment.Position;
            for (int j = 0; j < LEG_COUNT; j++) {
                Raylib.DrawLineEx(lastLegPos * GameWorld.TILE_SIZE, segment.LeftLegs[j] * GameWorld.TILE_SIZE, 4, color);
                lastLegPos = segment.LeftLegs[j];
            }
        }
    }

    public class WormSegment {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2[] LeftLegs { get; }
        public Vector2[] RightLegs { get; }

        public WormSegment(Vector2 position) {
            Position = position;
            Rotation = Random.Shared.NextAngle();
            LeftLegs = new Vector2[LEG_COUNT];
            RightLegs = new Vector2[LEG_COUNT];

            for (int i = 0; i < LEG_COUNT; i++) {
                LeftLegs[i] = Position + Random.Shared.NextVector(-MAX_LEG_DISTANCE, MAX_LEG_DISTANCE);
                RightLegs[i] = Position + Random.Shared.NextVector(-MAX_LEG_DISTANCE, MAX_LEG_DISTANCE);
            }
        }
    }
}
