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
    internal void GetTileTexture(ulong tileId, byte configuration, out SubTexture texture, out float rotation) {
        TileType tileType = GetTileType(tileId);

        GetConfigurationKey(configuration, out string configurationKey, out int rotValue);

        string[] tileTextureVariations = TileTextureAtlas.SubTextures.Keys.Where(k => k.StartsWith(tileType.Name) && k.EndsWith(configurationKey)).ToArray();

        bool hasVariations = tileTextureVariations.Length > 0;
        if (!hasVariations) {
            tileTextureVariations = TileTextureAtlas.SubTextures.Keys.Where(k => k.StartsWith(tileType.Name)).ToArray();
            rotValue = 0;
        }

        int variationIndex = Random.Shared.Next(tileTextureVariations.Length);

        string textureKey;
        if (hasVariations)
            textureKey = $"{tileType.Name}_{variationIndex}_{configurationKey}";
        else
            textureKey = $"{tileType.Name}_{variationIndex}_solo";

        texture = TileTextureAtlas.GetSubTexture(textureKey)!;
        rotation = rotValue * MathF.PI / 2f;
    }

    private static void GetConfigurationKey(byte configuration, out string configurationKey, out int rotation) {
        configurationKey = string.Empty;
        rotation = 0;
        switch (configuration) {
            case 0:
                configurationKey = "solo";
                rotation = 0;
                break;
            case 1:
                configurationKey = "end";
                rotation = 1;
                break;
            case 2:
                configurationKey = "end";
                rotation = 3;
                break;
            case 3:
                configurationKey = "straight";
                rotation = 1;
                break;
            case 4:
                configurationKey = "end";
                rotation = 2;
                break;
            case 5:
                configurationKey = "corner";
                rotation = 2;
                break;
            case 6:
                configurationKey = "corner";
                rotation = 3;
                break;
            case 7:
                configurationKey = "side";
                rotation = 3;
                break;
            case 8:
                configurationKey = "end";
                rotation = 0;
                break;
            case 9:
                configurationKey = "corner";
                rotation = 1;
                break;
            case 10:
                configurationKey = "corner";
                rotation = 0;
                break;
            case 11:
                configurationKey = "side";
                rotation = 1;
                break;
            case 12:
                configurationKey = "straight";
                rotation = 0;
                break;
            case 13:
                configurationKey = "side";
                rotation = 2;
                break;
            case 14:
                configurationKey = "side";
                rotation = 0;
                break;
            case 15:
                configurationKey = "center";
                rotation = 0;
                break;
            default:
                configurationKey = "solo";
                rotation = 0;
                break;
        }
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
