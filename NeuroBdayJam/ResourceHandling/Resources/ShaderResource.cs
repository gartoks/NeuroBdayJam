using Raylib_CsLo;
using System.Collections.Concurrent;

namespace NeuroBdayJam.ResourceHandling.Resources;
/// <summary>
/// Game resource for text.
/// </summary>
internal sealed class ShaderResource : GameResource<Shader> {
    /// <summary>
    /// Constructor for a new text resource.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="resourceRetriever"></param>
    internal ShaderResource(string key, Shader fallback, ResourceRetrieverDelegate resourceRetriever)
        : base(key, fallback, resourceRetriever) {
    }
}

internal sealed class ShaderResourceLoader : ResourceLoader<Shader, ShaderResource> {
    public ShaderResourceLoader(BlockingCollection<(string key, Type type)> resourceLoadingQueue)
        : base(resourceLoadingQueue) {
    }

    protected override bool ResourceExistsInternal(string key) {
        return ResourceManager.MainResourceFile.DoesShaderExist(key);
    }

    public override IReadOnlyList<string> GetResources() {
        return ResourceManager.MainResourceFile.GetShaderResources();
    }

    protected override Shader LoadResourceInternal(string key) {
        Shader? res = ResourceManager.MainResourceFile.LoadShader(key);
        return res ?? Fallback.Resource;
    }

    protected override void UnloadResourceInternal(ShaderResource resource) {
        if (resource.IsLoaded)
            Raylib.UnloadShader(resource.Resource);
    }
}