using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.World;
internal sealed class WorldTile : IEquatable<WorldTile?> {
    private GameWorld World { get; }

    public ulong Id { get; }
    public (int x, int y) Position { get; }
    public byte Configuration { get; }

    private float _NoiseValue { get; set; }
    public float NoiseValue {
        get => _NoiseValue;
        set {
            _NoiseValue = Math.Max(0, Math.Min(1, value));
        }
    }

    private SubTexture? Texture { get; set; }
    private float Rotation { get; set; }

    public WorldTile(GameWorld world, ulong tileId, (int x, int y) position, byte configuration) {
        World = world;
        Id = tileId;
        Position = position;
        Configuration = configuration;
        NoiseValue = 0;
    }

    internal void Update(float dT) {
        float currentNoise = NoiseValue;
        NoiseValue -= dT * 0.25f;
        float dNoise = currentNoise - NoiseValue;
    }

    internal void Render(float dT) {
        if (Texture == null) {
            World.Tileset.GetTileTexture(Id, Configuration, out SubTexture texture, out float rotation);
            Texture = texture;
            Rotation = rotation;
        }

        TileType tileType = World.Tileset.GetTileType(Id);

        Rectangle drawBounds = new Rectangle(Position.x * GameWorld.TILE_SIZE, Position.y * GameWorld.TILE_SIZE, GameWorld.TILE_SIZE, GameWorld.TILE_SIZE);

        Color tileColor = Id != 1 ? Raylib.WHITE : Raylib.DARKGRAY;
        Texture.Draw(drawBounds, Vector2.Zero, Rotation, tileColor);

        if (NoiseValue > 0) {
            SubTexture noiseTexture = World.MiscAtatlas.GetSubTexture("sound_marker")!;
            noiseTexture.Draw(drawBounds, Vector2.Zero, 0, Raylib.WHITE.ChangeAlpha((int)((0.5f * NoiseValue) * 255)));
        }

        if (Application.DRAW_DEBUG && tileType.Collider != null) {
            Rectangle boundsRect = new Rectangle((Position.x + tileType.Collider.Value.x) * GameWorld.TILE_SIZE, (Position.y + tileType.Collider.Value.y) * GameWorld.TILE_SIZE, tileType.Collider.Value.width * GameWorld.TILE_SIZE, tileType.Collider.Value.height * GameWorld.TILE_SIZE);
            Raylib.DrawRectangleLinesEx(boundsRect, 1, Raylib.LIME);
        }
    }

    public override bool Equals(object? obj) {
        return Equals(obj as WorldTile);
    }

    public bool Equals(WorldTile? other) {
        return other is not null &&
               Position.Equals(other.Position);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Position);
    }

    public static bool operator ==(WorldTile? left, WorldTile? right) => EqualityComparer<WorldTile>.Default.Equals(left, right);
    public static bool operator !=(WorldTile? left, WorldTile? right) => !(left == right);
}
