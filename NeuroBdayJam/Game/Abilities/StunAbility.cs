using NeuroBdayJam.Audio;
using NeuroBdayJam.Game.Entities;
using NeuroBdayJam.Game.Entities.Effects;
using NeuroBdayJam.Game.Entities.Enemies;
using System.Numerics;

namespace NeuroBdayJam.Game.Abilities;
internal sealed class StunAbility : Ability {
    private const float RADIUS = 5;

    private List<Enemy> StunnedEnemies { get; }

    public StunAbility()
        : base("Stun", 20, 3) {

        StunnedEnemies = new();
    }

    protected override void OnUse(Entity user) {
        AudioManager.PlaySound("ability_3");
        user.World.AddEntity(new StunEffect(user.Position));

        IEnumerable<Enemy> enemies = user.World!.AllEntities.OfType<Enemy>();
        IEnumerable<Enemy> enemiesInRange = enemies.Where(e => Vector2.Distance(e.Position, user.Position) < RADIUS);
        StunnedEnemies.AddRange(enemiesInRange);
        foreach (Enemy enemy in StunnedEnemies) {
            enemy.SetState(eEntityStates.Stunned);
        }

    }

    protected override void OnExpire(Entity user) {
        foreach (Enemy enemy in StunnedEnemies) {
            enemy.RemoveState(eEntityStates.Stunned);
        }
        StunnedEnemies.Clear();
    }

    protected override bool ShouldCancel(Entity user) {
        return false;
    }
}
