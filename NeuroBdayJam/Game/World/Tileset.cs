using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;

namespace NeuroBdayJam.Game.World;
internal sealed class Tileset : IDisposable {
    public string Name { get; }

    private IReadOnlyDictionary<ulong, TileType> TileTypes { get; }
    private TextureAtlas TileTextureAtlas { get; set; }

    private bool disposedValue;

    public Tileset(string name, IReadOnlySet<TileType> tileTypes, TextureAtlas tileTextureAtlas) {
        Name = name;
        TileTypes = tileTypes.ToDictionary(tt => tt.TileId, tt => tt);
        TileTextureAtlas = tileTextureAtlas;
    }

    public TileType GetTileType(ulong tileId) {
        if (!TileTypes.TryGetValue(tileId, out TileType? tileType))
            throw new ArgumentException($"Tileset {Name} does not contain a tile with id {tileId}.");

        return tileType;
    }

    /// <summary>
    /// Gets the texture for a tile. The configuration is used to get the correct texture variation (corner, side, etc).
    /// TODO: the configuration will be a bitmask that is set based on the surrounding tiles (so that walls can connect for example)
    /// </summary>
    /// <param name="tileId"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    internal SubTexture GetTileTexture(ulong tileId, byte configuration = 0) {
        TileType tileType = GetTileType(tileId);

        string[] tileTextureVariations = TileTextureAtlas.SubTextures.Keys.Where(k =>
            k.StartsWith(tileType.Name) && (
            (configuration == 0 && k.Split("_").Length == 2) ||
            (configuration > 0 && k.Split("_").Length == 3))).ToArray();

        int textureIndex = Random.Shared.Next(tileTextureVariations.Length);
        string textureKey = tileTextureVariations[textureIndex];

        return TileTextureAtlas.GetSubTexture(textureKey)!;

    }

    public void Unload() {
        Dispose();
    }

    private void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                Raylib.UnloadTexture(TileTextureAtlas.Texture);
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Tileset()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
