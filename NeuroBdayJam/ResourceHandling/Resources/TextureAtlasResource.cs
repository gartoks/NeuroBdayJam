﻿using Raylib_CsLo;
using System.Collections.Concurrent;
using System.Numerics;

namespace NeuroBdayJam.ResourceHandling.Resources;

public record TextureAtlas(Texture Texture, IReadOnlyDictionary<string, SubTexture> SubTextures) {
    internal SubTexture? GetSubTexture(string key) {
        if (!SubTextures.TryGetValue(key, out SubTexture? subTexture))
            return null;

        return subTexture;
    }

    internal void DrawAsBitmapFont(string text, float spacing, float height, Vector2 position, Vector2? pivot = null, Color? tint = null) {
        if (text.Length == 0)
            return;

        if (pivot == null)
            pivot = Vector2.Zero;

        List<SubTexture> subTextures = new();

        for (int i = 0; i < text.Length; i++) {
            char c = text[i];

            if (SubTextures.TryGetValue(c.ToString(), out SubTexture? subTexture))
                subTextures.Add(subTexture);
        }

        float maxH = subTextures.Select(st => st.Bounds.h).Max();
        float heightScale = height / maxH;

        float totalWidth = subTextures.Select(st => st.Bounds.w).Sum() * heightScale + (subTextures.Count - 1) * spacing;
        float x = position.X - totalWidth * pivot.Value.X;
        float y = position.Y - height * pivot.Value.Y;

        for (int i = 0; i < subTextures.Count; i++) {
            SubTexture subTexture = subTextures[i];

            float tx = subTexture.Bounds.x;
            float ty = subTexture.Bounds.y;
            float tw = subTexture.Bounds.w;
            float th = subTexture.Bounds.h;

            float w = heightScale * tw;
            float h = height;

            Raylib.DrawTexturePro(
                    Texture,
                    new Rectangle(tx, ty, tw, th),
                    new Rectangle(x, y, w, h),
                    Vector2.Zero,
                    0,
                    tint != null ? tint.Value : Raylib.WHITE);

            x += w + spacing;
        }
    }
}

public record SubTexture(string key, (int x, int y, int w, int h) Bounds, Texture AtlasTexture) : IDrawableResource {
    public float Width => Bounds.w;
    public float Height => Bounds.h;

    public void Draw(Rectangle bounds, Vector2? pivot = null, float rotation = 0, Color? tint = null) {
        if (pivot == null)
            pivot = Vector2.Zero;

        rotation *= RayMath.RAD2DEG;

        Raylib.DrawTexturePro(
                    AtlasTexture,
                    new Rectangle(Bounds.x + 0.05f, Bounds.y + 0.05f, Bounds.w - 0.1f, Bounds.h - 0.1f),
                    new Rectangle(bounds.x - bounds.width * (pivot.Value.X - 0.5f), bounds.y - bounds.height * (pivot.Value.Y - 0.5f), bounds.width, bounds.height),
                    new Vector2(bounds.width / 2f, bounds.height / 2f),
                    rotation,
                    tint != null ? tint.Value : Raylib.WHITE);
    }

    public void Draw(Vector2 position, Vector2 size, Vector2? pivot = null, float rotation = 0, Color? tint = null) {
        Rectangle r = new Rectangle(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f, size.X, size.Y);
        Draw(r, pivot, rotation, tint);
    }
}

/// <summary>
/// Game resource for npatch textures.
/// </summary>
internal sealed class TextureAtlasResource : GameResource<TextureAtlas> {
    /// <summary>
    /// Constructor for a new npatch texture resource.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fallback"></param>
    /// <param name="resourceRetriever"></param>
    internal TextureAtlasResource(string key, TextureAtlas fallback, ResourceRetrieverDelegate resourceRetriever)
        : base(key, fallback, resourceRetriever) {
    }
}

internal sealed class TextureAtlasResourceLoader : ResourceLoader<TextureAtlas, TextureAtlasResource> {
    public TextureAtlasResourceLoader(BlockingCollection<(string key, Type type)> resourceLoadingQueue)
        : base(resourceLoadingQueue) {
    }

    protected override bool ResourceExistsInternal(string key) {
        return ResourceManager.MainResourceFile.DoesTextureAtlasExist(key);
    }

    protected override TextureAtlas LoadResourceInternal(string key) {
        TextureAtlas? res = ResourceManager.MainResourceFile.LoadTextureAtlas(key);
        return res ?? Fallback.Resource;
    }

    public override IReadOnlyList<string> GetResources() {
        return ResourceManager.MainResourceFile.GetTextureAtlasResources();
    }

    protected override void UnloadResourceInternal(TextureAtlasResource resource) {
        if (resource.Resource.Texture.id != 0 && resource.Resource.Texture.id != Fallback.Resource.Texture.id)
            Raylib.UnloadTexture(resource.Resource.Texture);
    }
}
