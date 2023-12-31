using NeuroBdayJam.App;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class VedalTerminal : Entity {
    private const float INTERACTION_RADIUS = 2.5f;
    private const float INTERACTION_TIME = 1.0f;
    private const float COOLDOWN_TIME = 10.0f;
    private static Color ProgressColor { get; } = new Color(0, 255, 0, 255);

    public override Vector2 Facing => Vector2.Zero;

    public override float CollisionRadius => INTERACTION_RADIUS;

    private bool IsPlayerNear => (World!.Player.Position - Position).LengthSquared() <= (INTERACTION_RADIUS + World.Player.CollisionRadius) * (INTERACTION_RADIUS + World.Player.CollisionRadius);

    private float SecondsHeld { get; set; }
    private float CooldownTimeLeft { get; set; }

    public VedalTerminal(Vector2 position)
        : base("VedalTerminal", position) {

        ZIndex = 10;

        SecondsHeld = 0;
        CooldownTimeLeft = 0;
    }

    public override void Render(float dT) {
        SubTexture texture;
        if (IsPlayerNear)
            texture = World!.MiscAtlas.GetSubTexture("tutel_brain_on")!;
        else
            texture = World!.MiscAtlas.GetSubTexture("tutel_brain_off")!;

        texture.Draw(Position * GameWorld.TILE_SIZE, new Vector2(3 * GameWorld.TILE_SIZE, 3 * GameWorld.TILE_SIZE), new Vector2(0, 0), 0);

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
        if (IsPlayerNear) {
            if (Input.IsHotkeyDown(GameHotkeys.INTERACT)) {
                SecondsHeld += dT;
            } else {
                SecondsHeld = 0;
            }

            if (SecondsHeld >= INTERACTION_TIME && CooldownTimeLeft <= 0) {
                World.MemoryTracker.InternalizeMemories();
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
