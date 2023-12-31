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

    private float Time { get; set; }

    public WorldTile(GameWorld world, ulong tileId, (int x, int y) position, byte configuration) {
        World = world;
        Id = tileId;
        Position = position;
        Configuration = configuration;
        NoiseValue = 0;

        Time = Random.Shared.NextSingle(0, 1);
    }

    internal void Update(float dT) {
        NoiseValue -= dT * 0.25f;
    }

    internal void Render(float dT) {
        Time += dT;

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
            string noiseTextureName = Time % 1f > 0.5f ? "sound_marker_0" : "sound_marker_1";

            SubTexture noiseTexture = World.MiscAtlas.GetSubTexture(noiseTextureName)!;
            noiseTexture.Draw(drawBounds, Vector2.Zero, 0, Raylib.WHITE.ChangeAlpha((int)((0.015f + 0.049f * NoiseValue) * 255)));
        }
        if (Application.DRAW_DEBUG && tileType.Collider != null) {
            Rectangle boundsRect2 = new Rectangle(Position.x * GameWorld.TILE_SIZE, Position.y * GameWorld.TILE_SIZE, GameWorld.TILE_SIZE, GameWorld.TILE_SIZE);
            Raylib.DrawRectangleLinesEx(boundsRect2, 1, Raylib.GRAY);

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
