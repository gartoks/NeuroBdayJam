using System.Collections.Concurrent;

namespace NeuroBdayJam.ResourceHandling.Resources;
/// <summary>
/// Game resource for text.
/// </summary>
internal sealed class TextResource : GameResource<IReadOnlyDictionary<string, string>> {
    /// <summary>
    /// Constructor for a new text resource.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="resourceRetriever"></param>
    internal TextResource(string key, IReadOnlyDictionary<string, string> fallback, ResourceRetrieverDelegate resourceRetriever)
        : base(key, fallback, resourceRetriever) {
    }
}

internal sealed class TextResourceLoader : ResourceLoader<IReadOnlyDictionary<string, string>, TextResource> {
    public TextResourceLoader(BlockingCollection<(string key, Type type)> resourceLoadingQueue)
        : base(resourceLoadingQueue) {
    }

    protected override bool ResourceExistsInternal(string key) {
        return ResourceManager.MainResourceFile.DoesTextExist(key);
    }

    public override IReadOnlyList<string> GetResources() {
        return ResourceManager.MainResourceFile.GetTextResources();
    }

    protected override IReadOnlyDictionary<string, string> LoadResourceInternal(string key) {
        IReadOnlyDictionary<string, string>? res = ResourceManager.MainResourceFile.LoadText(key);
        return res ?? Fallback.Resource;
    }

    protected override void UnloadResourceInternal(TextResource resource) {
    }
}