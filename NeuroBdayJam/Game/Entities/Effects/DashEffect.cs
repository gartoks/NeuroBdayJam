using NeuroBdayJam.Game.World;
using NeuroBdayJam.Util;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Effects;
internal class DashEffect : Effect {
    private const float DURATION = 0.5f;
    private const int PARTICLE_COUNT = 10;
    private const float PARTICLE_SIZE = 0.2f;

    public override float CollisionRadius => 0;
    public override Vector2 Facing => Vector2.Zero;

    private Vector2 TargetPosition { get; }
    private float TargetRotation { get; }
    private float RemainingDuration { get; set; }

    private IReadOnlyList<Particle> Particles { get; }

    public DashEffect(Vector2 position, Vector2 targetPosition)
        : base("DashEffect", position) {

        TargetPosition = targetPosition;
        TargetRotation = Random.Shared.NextAngle();
        RemainingDuration = DURATION;

        Particles = Enumerable.Range(0, PARTICLE_COUNT)
            .Select(_ => new Particle(
                            Vector2.Normalize(Random.Shared.NextPointOnUnitSphere()) * Random.Shared.NextSingle(0.1f, 0.8f),
                            Random.Shared.NextAngle()))
            .ToList();
    }

    public override void Update(float dT) {
        RemainingDuration -= dT;

        if (RemainingDuration <= 0)
            IsDead = true;
    }

    public override void Render(float dT) {
        float t = (1 - RemainingDuration / DURATION);

        foreach (Particle particle in Particles) {
            Vector2 position = Position + particle.Facing * t;
            float rotation = particle.Rotation * t;
            Vector2 size = new Vector2(PARTICLE_SIZE, PARTICLE_SIZE) * (0.2f + t);
            RaylibUtil.DrawRectLines(position * GameWorld.TILE_SIZE, size * GameWorld.TILE_SIZE, rotation, 2, new Color(Random.Shared.Next(255), Random.Shared.Next(255), Random.Shared.Next(255), 255));

            Vector2 targetPosition = TargetPosition + particle.Facing * t;
            float targetRotation = TargetRotation + particle.Rotation * t;
            RaylibUtil.DrawRectLines(targetPosition * GameWorld.TILE_SIZE, size * GameWorld.TILE_SIZE, targetRotation, 2, new Color(Random.Shared.Next(255), Random.Shared.Next(255), Random.Shared.Next(255), 255));
        }

        Raylib.DrawLineEx(Position * GameWorld.TILE_SIZE, TargetPosition * GameWorld.TILE_SIZE, 2, new Color(Random.Shared.Next(255), Random.Shared.Next(255), Random.Shared.Next(255), 255));
    }

    private record Particle(Vector2 Facing, float Rotation);
}
