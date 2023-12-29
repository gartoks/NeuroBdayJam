using Raylib_CsLo;

namespace NeuroBdayJam.Game.World;
internal sealed class TileType : IEquatable<TileType?> {
    public ulong TileId { get; }
    public string Name { get; }
    public Rectangle? Collider { get; }

    public TileType(ulong tileId, string name, Rectangle? collider) {
        TileId = tileId;
        Name = name;
        Collider = collider;
    }

    public override bool Equals(object? obj) {
        return Equals(obj as TileType);
    }

    public bool Equals(TileType? other) {
        return other is not null &&
               TileId == other.TileId;
    }

    public override int GetHashCode() {
        return HashCode.Combine(TileId);
    }

    public static bool operator ==(TileType? left, TileType? right) => EqualityComparer<TileType>.Default.Equals(left, right);
    public static bool operator !=(TileType? left, TileType? right) => !(left == right);
}
