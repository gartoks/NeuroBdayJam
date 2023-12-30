using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Enemies;
internal abstract class Enemy : Entity {
    protected Enemy(string name, Vector2 position)
        : base(name, position) {
    }
}
