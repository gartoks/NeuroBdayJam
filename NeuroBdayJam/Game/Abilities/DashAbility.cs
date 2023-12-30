using NeuroBdayJam.Game.Entities;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Abilities;
internal sealed class DashAbility : Ability {

    public float DashDistance { get; }

    public DashAbility()
        : base("Dash", 5, 0.1f) {

        DashDistance = 3.0f;
    }

    protected override void OnUse(Entity user) {
        Vector2 facing = Vector2.Normalize(user.Facing);

        if (facing.LengthSquared() == 0)
            return;

        float distance = DashDistance;

        Vector2 dashTargetPosition;
        WorldTile? targetTile;
        do {
            dashTargetPosition = user.Position + facing * distance;
            targetTile = user.World!.GetTile(dashTargetPosition);

            distance -= 0.1f * DashDistance;
        } while (distance > 0 && (targetTile == null || targetTile.Id != 1));

        if (distance <= 0 || targetTile == null || targetTile.Id != 1)
            return;

        IReadOnlyList<Rectangle> colliders = user.World.GetSurroundingTileColliders(dashTargetPosition);
        Vector2 mtv = Collisions.ResolveCollisionCircleRects(dashTargetPosition, user.CollisionRadius, colliders);
        dashTargetPosition += mtv;

        user.Position = dashTargetPosition;
        user.SetState(eEntityStates.Hidden);
    }

    protected override void OnExpire(Entity user) {
        user.RemoveState(eEntityStates.Hidden);
    }

    protected override bool ShouldCancel(Entity user) {
        return false;
    }
}
