using NeuroBdayJam.Game.World;
using System.Numerics;

namespace NeuroBdayJam.Game.Entities;
internal sealed class NoiseSpawner : Entity {
    private const float RADIUS_GROWTH_SPEED = 2.5f;

    public override Vector2 Facing => Vector2.Zero;

    public override float CollisionRadius => 0;

    private float NoiseStrength { get; }
    private float CurrentRadius { get; set; }

    private HashSet<WorldTile> AffectedTiles { get; }


    public NoiseSpawner(Vector2 position, float noiseStrength)
        : base("Noise Spawner", position) {

        NoiseStrength = noiseStrength;
        CurrentRadius = 0;

        AffectedTiles = new HashSet<WorldTile>();
    }

    public override void Update(float dT) {
        base.Update(dT);

        float noiseValue = CalculateNoiseValue(CurrentRadius);

        if (noiseValue > 0) {
            HashSet<WorldTile> tiles = new HashSet<WorldTile>();
            for (float xi = -CurrentRadius; xi <= CurrentRadius; xi++) {
                for (float yi = -CurrentRadius; yi <= CurrentRadius; yi++) {
                    Vector2 offset = new Vector2(xi, yi);

                    if (offset.LengthSquared() > CurrentRadius * CurrentRadius)
                        continue;

                    Vector2 p = new Vector2(Position.X + xi, Position.Y + yi);
                    WorldTile? tile = World!.GetTile(p);

                    if (tile != null && tile.Id == 1 && !AffectedTiles.Contains(tile)) {
                        tiles.Add(tile);
                        AffectedTiles.Add(tile);
                    }
                }
            }

            foreach (WorldTile tile in tiles) {
                tile.NoiseValue += noiseValue;
            }
        } else {
            IsDead = true;
        }

        CurrentRadius += RADIUS_GROWTH_SPEED * dT;
    }

    private float CalculateNoiseValue(float radius) {
        float tmp = 0.1f / NoiseStrength / 2.5f * radius;
        return (-tmp * tmp + 1) * NoiseStrength;
    }
}
