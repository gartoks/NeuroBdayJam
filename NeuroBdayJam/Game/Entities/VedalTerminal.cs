using NeuroBdayJam.App;
using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class VedalTerminal : Entity {
    private const float INTERACTION_RADIUS = 0.7f;
    private const float SIZE = 0.5f;
    private const float INTERACTION_TIME = 1.0f;
    private const float COOLDOWN_TIME = 10.0f;
    private static Color Color { get; } = new Color(43, 199, 4, 255);
    private static Color ProgressColor { get; } = new Color(77, 77, 77, 255);

    public override float CollisionRadius => INTERACTION_RADIUS;

    private float SecondsHeld { get; set; }
    private float CooldownTimeLeft { get; set; }

    public override Vector2 Facing => Vector2.Zero;

    public VedalTerminal(Vector2 position)
        : base("VedalTerminal", position) {

        SecondsHeld = 0;
        CooldownTimeLeft = 0;
    }

    public override void Render(float dT) {
        Raylib.DrawRectangleV((Position - new Vector2(SIZE / 2)) * GameWorld.TILE_SIZE, new Vector2(SIZE) * GameWorld.TILE_SIZE, Color);

        if (Application.DRAW_DEBUG)
            Raylib.DrawCircleLines((int)(Position.X * GameWorld.TILE_SIZE), (int)(Position.Y * GameWorld.TILE_SIZE), INTERACTION_RADIUS * GameWorld.TILE_SIZE, Raylib.ORANGE);

        if (SecondsHeld != 0.0 && CooldownTimeLeft <= 0.0) {
            float progress = SecondsHeld / INTERACTION_TIME;
            Raylib.DrawCircleSectorLines(
                (World!.Player.Position + new Vector2(0, -1.35f)) * GameWorld.TILE_SIZE, 20, 180, 180 - 360 * progress,
                (int)(72 * progress) + 1, ProgressColor);
        }
    }

    public override void Update(float dT) {
        if ((World!.Player.Position - Position).LengthSquared() <= (INTERACTION_RADIUS + World.Player.CollisionRadius) * (INTERACTION_RADIUS + World.Player.CollisionRadius)) {
            if (Input.IsHotkeyDown(GameHotkeys.INTERACT)) {
                SecondsHeld += dT;
            } else {
                SecondsHeld = 0;
            }

            if (SecondsHeld >= INTERACTION_TIME && CooldownTimeLeft <= 0) {
                MemoryTracker.InternalizeMemories();
                SecondsHeld = 0;
                CooldownTimeLeft = COOLDOWN_TIME;
            }
        } else {
            SecondsHeld = 0;
        }
        if (CooldownTimeLeft > 0) {
            CooldownTimeLeft -= dT;
        }

    }
}
