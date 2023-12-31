using NeuroBdayJam.Game.Entities;

namespace NeuroBdayJam.Game.Abilities;
internal abstract class Ability {
    public string Name { get; }

    public float Cooldown { get; }
    public float CooldownRemaining { get; private set; }
    public bool IsReady => CooldownRemaining <= 0;
    public float RechargePercentage => CooldownRemaining / Cooldown;

    public float Duration { get; }
    public float DurationRemaining { get; private set; }
    public bool IsActive => DurationRemaining > 0;

    private Entity? User { get; set; }

    protected Ability(string name, float cooldown, float duration) {
        Name = name;
        Cooldown = cooldown;
        CooldownRemaining = 0;
        Duration = duration;
        DurationRemaining = 0;
    }

    internal void Update(float dT) {
        if (CooldownRemaining > 0)
            CooldownRemaining = Math.Max(0, CooldownRemaining - dT);

        if (DurationRemaining > 0) {
            DurationRemaining = Math.Max(0, DurationRemaining - dT);

            if (DurationRemaining <= 0) {
                OnExpire(User!);
                User = null;
            } else if (ShouldCancel(User!))
                Cancel();
        }
    }

    public void Use(Entity user) {
        if (!IsReady)
            return;

        CooldownRemaining = Cooldown;
        DurationRemaining = Duration;
        User = user;
        OnUse(user);
    }

    public void Cancel() {
        if (!IsActive)
            return;

        DurationRemaining = 0;
        OnExpire(User!);
        User = null;
    }

    protected abstract void OnUse(Entity user);
    protected abstract void OnExpire(Entity user);
    protected abstract bool ShouldCancel(Entity user);
}
