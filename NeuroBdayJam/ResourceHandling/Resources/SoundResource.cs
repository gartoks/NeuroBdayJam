using Raylib_CsLo;
using System.Collections.Concurrent;

namespace NeuroBdayJam.ResourceHandling.Resources;

/// <summary>
/// Game resource for sounds.
/// </summary>
internal sealed class SoundResource : GameResource<Sound> {
    /// <summary>
    /// Constructor for a new sound resource.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fallback"></param>
    /// <param name="resourceRetriever"></param>
    internal SoundResource(string key, Sound fallback, ResourceRetrieverDelegate resourceRetriever)
        : base(key, fallback, resourceRetriever) {
    }
}

internal sealed class SoundResourceLoader : ResourceLoader<Sound, SoundResource> {
    public SoundResourceLoader(BlockingCollection<(string key, Type type)> resourceLoadingQueue)
        : base(resourceLoadingQueue) {
    }

    protected override bool ResourceExistsInternal(string key) {
        return ResourceManager.MainTheme.DoesSoundExist(key);
    }

    protected override Sound LoadResourceInternal(string key) {
        Sound? res = ResourceManager.MainTheme.LoadSound(key) ?? ResourceManager.MainResourceFile.LoadSound(key);
        return res ?? Fallback.Resource;
    }

    protected override void UnloadResourceInternal(SoundResource resource) {
    }
}