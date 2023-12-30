using System.Numerics;

namespace NeuroBdayJam.Game.Entities.Effects;
internal abstract class Effect : Entity {
    protected Effect(string name, Vector2 position)
        : base(name, position) {
    }
}
