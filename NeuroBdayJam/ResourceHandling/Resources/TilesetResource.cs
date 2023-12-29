using NeuroBdayJam.Game.World;
using System.Collections.Concurrent;

namespace NeuroBdayJam.ResourceHandling.Resources;

/// <summary>
/// Game resource for npatch textures.
/// </summary>
internal sealed class TilesetResource : GameResource<Tileset> {
    /// <summary>
    /// Constructor for a new npatch texture resource.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fallback"></param>
    /// <param name="resourceRetriever"></param>
    internal TilesetResource(string key, Tileset fallback, ResourceRetrieverDelegate resourceRetriever)
        : base(key, fallback, resourceRetriever) {
    }
}

internal sealed class TilesetResourceLoader : ResourceLoader<Tileset, TilesetResource> {
    public TilesetResourceLoader(BlockingCollection<(string key, Type type)> resourceLoadingQueue)
        : base(resourceLoadingQueue) {
    }

    protected override bool ResourceExistsInternal(string key) {
        return ResourceManager.MainResourceFile.DoesTilesetExist(key);
    }

    public override IReadOnlyList<string> GetResources() {
        return ResourceManager.MainResourceFile.GetTilesetResources();
    }

    protected override Tileset LoadResourceInternal(string key) {
        Tileset? res = ResourceManager.MainResourceFile.LoadTileset(key);
        return res ?? Fallback.Resource;
    }

    protected override void UnloadResourceInternal(TilesetResource resource) {
        if (resource.IsLoaded)
            resource.Resource.Unload();
    }
}
