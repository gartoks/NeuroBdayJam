﻿using Raylib_CsLo;
using System.Collections.Concurrent;

namespace NeuroBdayJam.ResourceHandling.Resources;
/// <summary>
/// Game resource for colors.
/// </summary>
internal sealed class ColorResource : GameResource<Color> {
    public static ColorResource WHITE { get; } = new ColorResource("white", Raylib.WHITE, _ => Raylib.WHITE);
    /// <summary>
    /// Constructor for a new color resource.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="resourceRetriever"></param>
    internal ColorResource(string key, Color fallback, ResourceRetrieverDelegate resourceRetriever)
        : base(key, fallback, resourceRetriever) {
    }
}

internal sealed class ColorResourceLoader : ResourceLoader<Color, ColorResource> {
    public ColorResourceLoader(BlockingCollection<(string key, Type type)> resourceLoadingQueue)
        : base(resourceLoadingQueue) {
    }

    protected override bool ResourceExistsInternal(string key) {
        return ResourceManager.MainResourceFile.DoesColorExist(key);
    }

    public override IReadOnlyList<string> GetResources() {
        return ResourceManager.MainResourceFile.GetColorResources();
    }

    protected override Color LoadResourceInternal(string key) {
        Color? res = ResourceManager.MainResourceFile.GetColor(key);
        return res ?? Fallback.Resource;
    }

    protected override void UnloadResourceInternal(ColorResource resource) {
    }
}