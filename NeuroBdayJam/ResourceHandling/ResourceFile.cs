using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;

namespace NeuroBdayJam.ResourceHandling;
/// <summary>
/// Class for one set of game resources. Doesn't cache anything.
/// </summary>
internal sealed class ResourceFile : IDisposable, IEquatable<ResourceFile?> {
    public string Name { get; }

    /// <summary>
    /// The file path to the asset file.
    /// </summary>
    private string ResourceFileFilePath { get; }

    /// <summary>
    /// A mapping of color name to colors.
    /// </summary>
    private Dictionary<string, Color> Colors { get; }

    /// <summary>
    /// The zip archive containing all assets.
    /// </summary>
    private ZipArchive? ResourceFileArchive { get; set; }

    /// <summary>
    /// A collection of all music data from this theme.
    /// This has to exist because Raylib.loadMusicStreamFromMemory
    /// streams in the music while playing. This just keeps it from
    /// being garbage collected.
    /// </summary>
    private List<byte[]> _MusicBuffers;

    /// <summary>
    /// Flag indicating whether the theme was loaded.
    /// </summary>
    private bool WasLoaded { get; set; }

    private bool disposedValue;

    /// <summary>
    /// Constructor to load a theme from disk.
    /// </summary>
    internal ResourceFile(string resourceFileFilePath) {
        Name = Path.GetFileNameWithoutExtension(resourceFileFilePath);

        ResourceFileFilePath = resourceFileFilePath;
        Colors = new Dictionary<string, Color>();
        _MusicBuffers = new();

        WasLoaded = false;
    }

    internal void Load() {
        if (WasLoaded)
            throw new InvalidOperationException("Resource file was already loaded.");

        Log.WriteLine($"Loading resource file {Name}");
        MemoryStream ms = new MemoryStream();
        using FileStream fs = new FileStream(ResourceFileFilePath, FileMode.Open);

        fs.CopyTo(ms);
        ms.Position = 0;

        ResourceFileArchive = new ZipArchive(ms, ZipArchiveMode.Read);

        ZipArchiveEntry? colorEntry = ResourceFileArchive.GetEntry("colors.json");
        if (colorEntry == null) {
            Log.WriteLine($"Resource file {ResourceFileFilePath} doesn't contain colors.");
            return;
        }

        StreamReader colorStreamReader = new StreamReader(colorEntry.Open());
        Dictionary<string, int[]>? colors = JsonSerializer.Deserialize<Dictionary<string, int[]>>(colorStreamReader.ReadToEnd());
        if (colors == null) {
            Log.WriteLine($"colors.json in resource file {ResourceFileFilePath} has a wrong format.");
            return;
        }

        foreach (KeyValuePair<string, int[]> entry in colors) {
            Colors[entry.Key] = new Color((byte)entry.Value[0], (byte)entry.Value[1], (byte)entry.Value[2], (byte)entry.Value[3]);
        }

        WasLoaded = true;
    }

    internal void Unload() {

    }

    internal bool DoesColorExist(string key) {
        return Colors.ContainsKey(key);
    }

    internal IReadOnlyList<string> GetColorResources() {
        return Colors.Keys.ToList();
    }

    /// <summary>
    /// Tries to get a color to the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    internal Color? GetColor(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        if (!Colors.ContainsKey(key)) {
            return null;
        }

        return Colors[key];
    }

    internal bool DoesFontExist(string key) {
        string path = $"Fonts/{key}.ttf";
        if (ResourceFileArchive!.GetEntry(path) != null)
            return true;

        path = $"Fonts/{key}.otf";
        return ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetFontResources() {
        return ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Fonts/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
    }

    /// <summary>
    /// Tries to load a font from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public Font? LoadFont(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        string path = $"Fonts/{key}.ttf";
        ZipArchiveEntry? zippedFont = ResourceFileArchive!.GetEntry(path);

        if (zippedFont == null) {
            path = $"Fonts/{key}.otf";
            zippedFont = ResourceFileArchive!.GetEntry(path);
        }

        if (zippedFont == null) {
            Log.WriteLine($"Font {key} doesn't exist in this resource file");
            return null;
        }

        using Stream fontStream = zippedFont.Open();
        byte[] fontData;
        using (MemoryStream ms = new MemoryStream()) {
            fontStream.CopyTo(ms);
            fontData = ms.ToArray();
        }

        Font font;
        unsafe {
            fixed (byte* fontPtr = fontData) {
                font = Raylib.LoadFontFromMemory(".ttf", fontPtr, fontData.Length, 200, null, 0);
            }
        }

        if (font.texture.id == 0) {
            Log.WriteLine($"Failed to load font {key} from {path}");
            return null;
        }
        return font;
    }

    internal bool DoesTextureExist(string key) {
        string path = $"Textures/{key}.png";
        return ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetTextureResources() {
        return ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Textures/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
    }

    /// <summary>
    /// Tries to load a texture from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public Texture? LoadTexture(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        string path = $"Textures/{key}.png";
        ZipArchiveEntry? zippedTexture = ResourceFileArchive!.GetEntry(path);

        if (zippedTexture == null) {
            Log.WriteLine($"Texture {key} doesn't exist in this resource file");
            return null;
        }

        using Stream textureStream = zippedTexture.Open();
        byte[] textureData;
        using (MemoryStream ms = new MemoryStream()) {
            textureStream.CopyTo(ms);
            ms.Position = 0;
            textureData = ms.ToArray();
        }

        Texture texture;
        unsafe {
            fixed (byte* texturePtr = textureData) {
                texture = Raylib.LoadTextureFromImage(Raylib.LoadImageFromMemory(".png", texturePtr, textureData.Length));
            }
        }

        if (texture.id == 0) {
            Log.WriteLine($"Failed to load texture {key} from {path}");
            return null;
        }
        return texture;
    }

    internal bool DoesSoundExist(string key) {
        string path = $"Sounds/{key}.wav";
        return ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetSoundResources() {
        return ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Sounds/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
    }

    /// <summary>
    /// Tries to load a sound from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public Sound? LoadSound(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        string path = $"Sounds/{key}.wav";
        ZipArchiveEntry? zippedSound = ResourceFileArchive!.GetEntry(path);

        if (zippedSound == null) {
            Log.WriteLine($"Sound {key} doesn't exist in this resource file");
            return null;
        }

        using Stream soundStream = zippedSound.Open();
        byte[] soundData;
        using (MemoryStream ms = new MemoryStream()) {
            soundStream.CopyTo(ms);
            ms.Position = 0;
            soundData = ms.ToArray();
        }

        Sound sound;
        unsafe {
            fixed (byte* soundPtr = soundData) {
                sound = Raylib.LoadSoundFromWave(Raylib.LoadWaveFromMemory(".wav", soundPtr, soundData.Length));
            }
        }

        return sound;
    }

    internal bool DoesMusicExist(string key) {
        string path = $"Music/{key}.mp3";
        return ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetMusicResources() {
        return ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Music/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
    }

    /// <summary>
    /// Tries to load a music from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public Music? LoadMusic(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        string path = $"Music/{key}.mp3";
        ZipArchiveEntry? zippedSound = ResourceFileArchive!.GetEntry(path);

        if (zippedSound == null) {
            Log.WriteLine($"Music {key} doesn't exist in this resource file");
            return null;
        }

        using Stream musicStream = zippedSound.Open();
        byte[] musicData;
        using (MemoryStream ms = new MemoryStream()) {
            musicStream.CopyTo(ms);
            ms.Position = 0;
            musicData = ms.ToArray();
        }

        Music music;
        unsafe {
            fixed (byte* soundPtr = musicData) {
                music = Raylib.LoadMusicStreamFromMemory(".mp3", soundPtr, musicData.Length);
            }
        }

        // force the data to stay alive until the theme changes.
        _MusicBuffers.Add(musicData);
        music.looping = false;

        return music;
    }

    internal bool DoesTextExist(string key) {
        string path = $"Texts/{key}.json";
        return ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetTextResources() {
        return ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Texts/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
    }

    /// <summary>
    /// Tries to get a text to the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    internal IReadOnlyDictionary<string, string>? LoadText(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        string path = $"Texts/{key}.json";
        ZipArchiveEntry? zippedText = ResourceFileArchive!.GetEntry(path);

        if (zippedText == null) {
            Log.WriteLine($"Text {key} doesn't exist in this resource file");
            return null;
        }

        using Stream textStream = zippedText.Open();
        Dictionary<string, string>? dict = JsonSerializer.Deserialize<Dictionary<string, string>>(textStream);

        return dict;
    }

    internal bool DoesNPatchTextureExist(string key) {
        string path = $"Textures/NPatchData/{key}.json";
        return DoesTextureExist(key) && ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetNPatchTextureResources() {
        List<string> atlasData = ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Textures/NPatchData/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
        List<string> textureData = ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Textures/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();

        return atlasData.Intersect(textureData).ToList();
    }

    /// <summary>
    /// Tries to load a NPatchTexture from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public NPatchTexture? LoadNPatchTexture(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        Texture texture = (Texture)LoadTexture(key);

        string path = $"Textures/NPatchData/{key}.json";
        ZipArchiveEntry? zippedText = ResourceFileArchive!.GetEntry(path);

        if (zippedText == null) {
            Log.WriteLine($"NPatchData {key} doesn't exist in this resource file");
            return null;
        }

        using Stream textStream = zippedText.Open();
        Dictionary<string, int>? dict = JsonSerializer.Deserialize<Dictionary<string, int>>(textStream);

        if (dict == null)
            return null;

        return new NPatchTexture(texture, dict["left"], dict["right"], dict["top"], dict["bottom"]);
    }

    internal bool DoesTextureAtlasExist(string key) {
        string path = $"Textures/TextureAtlasData/{key}.json";
        return DoesTextureExist(key) && ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetTextureAtlasResources() {
        List<string> atlasData = ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Textures/TextureAtlasData/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();
        List<string> textureData = ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Textures/")).Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();

        return atlasData.Intersect(textureData).ToList();
    }

    /// <summary>
    /// Tries to load a TextureAtlas from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public TextureAtlas? LoadTextureAtlas(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        Texture texture = (Texture)LoadTexture(key);

        string path = $"Textures/TextureAtlasData/{key}.json";
        ZipArchiveEntry? zippedText = ResourceFileArchive!.GetEntry(path);

        if (zippedText == null) {
            Log.WriteLine($"TextureAtlasData {key} doesn't exist in this resource file");
            return null;
        }

        using Stream textStream = zippedText.Open();
        Dictionary<string, string>? dict = JsonSerializer.Deserialize<Dictionary<string, string>>(textStream);

        if (dict == null)
            return null;

        Dictionary<string, SubTexture> subTextures = new();
        foreach (KeyValuePair<string, string> item in dict) {
            string subTextureKey = item.Key;
            string[] components = item.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (components.Length != 4) {
                Log.WriteLine($"TextureAtlasData {key} has an invalid format.", eLogType.Error);
                continue;
            }

            if (!int.TryParse(components[0], CultureInfo.InvariantCulture, out int x) ||
                !int.TryParse(components[1], CultureInfo.InvariantCulture, out int y) ||
                !int.TryParse(components[2], CultureInfo.InvariantCulture, out int w) ||
                !int.TryParse(components[3], CultureInfo.InvariantCulture, out int h)) {
                Log.WriteLine($"TextureAtlasData {key} has an invalid format.", eLogType.Error);
                continue;
            }

            subTextures[subTextureKey] = new SubTexture(subTextureKey, (x, y, w, h), texture);
        }

        return new TextureAtlas(texture, subTextures);
    }

    internal bool DoesTilesetExist(string key) {
        string path = $"Tilesets/{key}/";
        return DoesTextureExist(key) && ResourceFileArchive!.GetEntry(path) != null;
    }

    internal IReadOnlyList<string> GetTilesetResources() {
        List<string> tilesetData = ResourceFileArchive!.Entries
            .Where(e => e.FullName.StartsWith("Tilesets/"))
            .Select(e => Path.GetFileName(Path.GetDirectoryName(e.FullName)!))
            .Distinct().ToList();
        List<string> textureData = ResourceFileArchive!.Entries
            .Where(e => e.FullName.StartsWith("Textures/") && Path.GetFileNameWithoutExtension(e.FullName).StartsWith("tileset_"))
            .Select(e => Path.GetFileNameWithoutExtension(e.FullName).Replace("tileset_", ""))
            .Distinct().ToList();

        return tilesetData.Intersect(textureData).ToList();
    }

    /// <summary>
    /// Tries to load a Tileset from the zip archive.
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="InvalidOperationException">Thrown if the resource file was not loaded.</exception>
    public Tileset? LoadTileset(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        TextureAtlas tileTextureAtlas = LoadTextureAtlas($"tileset_{key}")!;

        string path = $"Tilesets/{key}/";
        if (!ResourceFileArchive!.Entries.Any(e => e.FullName.StartsWith(path))) {
            Log.WriteLine($"Tileset {key} doesn't exist in this resource file.");
            return null;
        }

        HashSet<TileType> tileTypes = new();

        IEnumerable<ZipArchiveEntry> tileTypeEntries = ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith(path));
        foreach (ZipArchiveEntry tileTypeEntry in tileTypeEntries) {
            Stream tileTypeStream = tileTypeEntry.Open();
            Dictionary<string, string>? dict = JsonSerializer.Deserialize<Dictionary<string, string>>(tileTypeStream);
            tileTypeStream.Dispose();

            if (dict == null)
                return null;

            TileType? tileType = ParseTileType(dict);

            if (tileType == null)
                return null;

            tileTypes.Add(tileType);
        }

        return new Tileset(key, tileTypes, tileTextureAtlas);
    }

    internal bool DoesShaderExist(string key) {
        string fragmentShaderPath = $"Shaders/{key}.fs";
        return ResourceFileArchive!.GetEntry(fragmentShaderPath) != null;
    }

    internal IReadOnlyList<string> GetShaderResources() {
        List<string> fragmentShaderData = ResourceFileArchive!.Entries.Where(e => e.FullName.StartsWith("Shaders/") && Path.GetExtension(e.FullName) == ".fs").Select(e => Path.GetFileNameWithoutExtension(e.FullName)).ToList();

        return fragmentShaderData.ToList();
    }

    public Shader? LoadShader(string key) {
        if (!WasLoaded)
            throw new InvalidOperationException("Resource file was not loaded.");

        string vertexShaderPath = $"Shaders/{key}.vs";
        ZipArchiveEntry? vertexShaderEntry = ResourceFileArchive!.GetEntry(vertexShaderPath);

        string fragmentShaderPath = $"Shaders/{key}.fs";
        ZipArchiveEntry? fragmentShaderEntry = ResourceFileArchive!.GetEntry(fragmentShaderPath);

        if (fragmentShaderEntry == null) {
            Log.WriteLine($"Shader {key} doesn't exist in this resource file");
            return null;
        }

        string? vertexShaderSource = null;
        if (vertexShaderEntry != null) {
            using Stream vertexShaderStream = vertexShaderEntry.Open();
            StreamReader vsr = new StreamReader(vertexShaderStream);
            vertexShaderSource = vsr.ReadToEnd();
        }

        using Stream fragmentShaderStream = fragmentShaderEntry.Open();
        StreamReader fsr = new StreamReader(fragmentShaderStream);
        string fragmentShaderSource = fsr.ReadToEnd();

        Shader shader = Raylib.LoadShaderFromMemory(vertexShaderSource, fragmentShaderSource);

        if (shader.id == 0) {
            Log.WriteLine($"Failed to load shader {key} from {key}");
            return null;
        }
        return shader;
    }

    private TileType? ParseTileType(IReadOnlyDictionary<string, string> dict) {
        if (!dict.TryGetValue("id", out string? idStr) || !ulong.TryParse(idStr, out ulong id))
            return null;

        if (!dict.TryGetValue("name", out string? name))
            return null;

        if (!dict.TryGetValue("collider", out string? colliderStr))
            return null;

        Rectangle? bounds;
        if (string.IsNullOrEmpty(colliderStr))
            bounds = null;
        else {
            string[] comps = colliderStr.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (comps.Length != 4)
                return null;

            if (!float.TryParse(comps[0], CultureInfo.InvariantCulture, out float x) ||
                !float.TryParse(comps[1], CultureInfo.InvariantCulture, out float y) ||
                !float.TryParse(comps[2], CultureInfo.InvariantCulture, out float w) ||
                !float.TryParse(comps[3], CultureInfo.InvariantCulture, out float h))
                return null;

            bounds = new Rectangle(x, y, w, h);
        }

        return new TileType(id, name, bounds);
    }

    private void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                ResourceFileArchive?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Resource file()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override bool Equals(object? obj) => Equals(obj as ResourceFile);
    public bool Equals(ResourceFile? other) => other is not null && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(Name);

    public static bool operator ==(ResourceFile? left, ResourceFile? right) => EqualityComparer<ResourceFile>.Default.Equals(left, right);
    public static bool operator !=(ResourceFile? left, ResourceFile? right) => !(left == right);
}
