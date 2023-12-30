using NeuroBdayJam.Game.Entities;

namespace NeuroBdayJam.Game.Abilities;
internal sealed class CamouflageAbility : Ability {

    public CamouflageAbility()
        : base("Camouflage", 30, 4) {
    }

    protected override void OnUse(Entity user) {
        user.SetState(eEntityStates.Hidden);
    }

    protected override void OnExpire(Entity user) {
        user.RemoveState(eEntityStates.Hidden);
    }

    protected override bool ShouldCancel(Entity user) {
        return user.HasMoved;
    }
}
