using NeuroBdayJam.App;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace NeuroBdayJam.ResourceHandling;
/// <summary>
/// Class for managing the game's resources. Handles loading and caching of resources.
/// </summary>
internal static class ResourceManager {
    /// <summary>
    /// Time in milliseconds to wait for a resource to be needed to be loaded before continuing the frame.
    /// </summary>
    private const int RESOURCE_LOADING_TIMEOUT = 0;
    /// <summary>
    /// Thread-safe queue of resources to be loaded.
    /// </summary>
    private static BlockingCollection<(string key, Type type)> ResourceLoadingQueue { get; }

    public static ColorResourceLoader ColorLoader { get; }
    public static FontResourceLoader FontLoader { get; }
    public static TextureResourceLoader TextureLoader { get; }
    public static SoundResourceLoader SoundLoader { get; }
    public static MusicResourceLoader MusicLoader { get; }
    public static TextResourceLoader TextLoader { get; }
    public static NPatchTextureResourceLoader NPatchTextureLoader { get; }
    public static TextureAtlasResourceLoader TextureAtlasLoader { get; }
    public static TilesetResourceLoader TilesetLoader { get; }

    /// <summary>
    /// Base theme.
    /// </summary>
    public static ResourceFile MainResourceFile { get; }

    /// <summary>
    /// Static constructor to initialize the resource loading queue and other required properties.
    /// </summary>
    static ResourceManager() {
        ResourceLoadingQueue = new BlockingCollection<(string key, Type type)>();

        ColorLoader = new(ResourceLoadingQueue);
        FontLoader = new(ResourceLoadingQueue);
        TextureLoader = new(ResourceLoadingQueue);
        SoundLoader = new(ResourceLoadingQueue);
        MusicLoader = new(ResourceLoadingQueue);
        TextLoader = new(ResourceLoadingQueue);
        NPatchTextureLoader = new(ResourceLoadingQueue);
        TextureAtlasLoader = new(ResourceLoadingQueue);
        TilesetLoader = new(ResourceLoadingQueue);

        MainResourceFile = new ResourceFile(Files.GetResourceFilePath("Main.dat"));
    }

    /// <summary>
    /// Used to initialize the resource manager. Currently does nothing.
    /// </summary>
    internal static void Initialize() {
    }

    /// <summary>
    /// Loads default resources.
    /// </summary>
    internal static void Load() {
        Image image = Raylib.GenImageColor(1, 1, Raylib.BLANK);
        Texture fallbackTexture = Raylib.LoadTextureFromImage(image);
        TextureAtlas fallbackTextureAtlas = new TextureAtlas(fallbackTexture, new Dictionary<string, SubTexture>());

        MainResourceFile.Load();

        ColorLoader.Load(Raylib.WHITE);
        FontLoader.Load(Raylib.GetFontDefault());
        TextureLoader.Load(fallbackTexture);
        SoundLoader.Load(new Sound());
        MusicLoader.Load(new Music());
        TextLoader.Load(new Dictionary<string, string>());
        NPatchTextureLoader.Load(new NPatchTexture(fallbackTexture, 0, 1, 0, 1));
        TextureAtlasLoader.Load(fallbackTextureAtlas);
        TilesetLoader.Load(new Tileset("__FALLBACK__", new HashSet<TileType>(), fallbackTextureAtlas));
    }

    /// <summary>
    /// Unloads all resources.
    /// </summary>
    internal static void Unload() {
        MainResourceFile?.Unload();
        MainResourceFile?.Dispose();
    }

    /// <summary>
    /// Clears the resource cache and reloads everything.
    /// </summary>
    private static void ReloadResources() {
        while (ResourceLoadingQueue.TryTake(out _)) ;

        Log.WriteLine("Reloading resources.");
        ColorLoader.ReloadAll();
        FontLoader.ReloadAll();
        TextureLoader.ReloadAll();
        SoundLoader.ReloadAll();
        MusicLoader.ReloadAll();
        TextLoader.ReloadAll();
        NPatchTextureLoader.ReloadAll();
        TextureAtlasLoader.ReloadAll();
        TilesetLoader.ReloadAll();
    }

    /// <summary>
    /// Called every frame. Checks the resource loading queue and loads resources if needed.
    /// </summary>
    internal static void Update() {
        while (ResourceLoadingQueue.TryTake(out (string key, Type type) resource, RESOURCE_LOADING_TIMEOUT)) {
            //Log.WriteLine($"Loading resource {resource.key} of type {resource.type}");
            LoadResource(resource.key, resource.type);
        }
    }

    /// <summary>
    /// Loads a resource from the given key and type.
    /// </summary>
    /// <param name="key">The key of the resource.</param>
    /// <param name="type">The type of the raylib resource type</param>
    private static void LoadResource(string key, Type type) {
        if (type == typeof(Color)) {
            ColorLoader.LoadResource(key);
        } else if (type == typeof(Font)) {
            FontLoader.LoadResource(key);
        } else if (type == typeof(Texture)) {
            TextureLoader.LoadResource(key);
        } else if (type == typeof(Sound)) {
            SoundLoader.LoadResource(key);
        } else if (type == typeof(Music)) {
            MusicLoader.LoadResource(key);
        } else if (type == typeof(IReadOnlyDictionary<string, string>)) {
            TextLoader.LoadResource(key);
        } else if (type == typeof(NPatchTexture)) {
            NPatchTextureLoader.LoadResource(key);
        } else if (type == typeof(TextureAtlas)) {
            TextureAtlasLoader.LoadResource(key);
        } else if (type == typeof(Tileset)) {
            TilesetLoader.LoadResource(key);
        } else {
            Debug.WriteLine($"Resource type {type} is not supported");
        }
    }

    public static void WaitForLoading() {
        while (ResourceLoadingQueue.Count != 0)
            Thread.Sleep(5);
    }
}
