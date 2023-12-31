using NeuroBdayJam.Game.World;
using NeuroBdayJam.Util;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Effects;
internal class StunEffect : Effect {
    private const float DURATION = 0.5f;
    private const int PARTICLE_COUNT = 18;
    private const float PARTICLE_SIZE = 0.2f;

    public override float CollisionRadius => 0;
    public override Vector2 Facing => Vector2.Zero;

    private float RemainingDuration { get; set; }

    private IReadOnlyList<Particle> Particles { get; }

    public StunEffect(Vector2 position)
        : base("StunEffect", position) {

        RemainingDuration = DURATION;

        Particles = Enumerable.Range(0, PARTICLE_COUNT)
            .Select(i => new Particle(
                            new Vector2(2.5f, 0).Rotate(MathF.Tau * i / PARTICLE_COUNT),
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
            RaylibUtil.DrawRectLines(position * GameWorld.TILE_SIZE, size * GameWorld.TILE_SIZE, rotation, 2, Raylib.WHITE);
        }
    }

    private record Particle(Vector2 Facing, float Rotation);
}
