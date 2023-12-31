using NeuroBdayJam.Audio;
using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.Game.Scenes;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;

namespace NeuroBdayJam.Game;
/// <summary>
/// Static class to control the main game. Handles the game's scenes.
/// </summary>
public static class GameManager {
    /// <summary>
    /// The currently active scene.
    /// </summary>
    private static Scene Scene { get; set; }
    /// <summary>
    /// Flag indicating if the scene was loaded after setting a new scene.
    /// </summary>
    private static bool WasSceneLoaded { get; set; }
    /// <summary>
    /// Lock to prevent trying to change scene while rendering.
    /// </summary>
    private static object SceneLock = new();

    private static IReadOnlyList<MusicResource> Music { get; set; }
    private static bool WasMusicQueued { get; set; }

    static GameManager() {
    }

    /// <summary>
    /// Initializes the game. Creates the initially loaded scene.
    /// </summary>
    internal static void Initialize() {
        WasSceneLoaded = false;
        WasMusicQueued = false;

        GuiManager.Initialize();
    }

    /// <summary>
    /// Loads the game. Loads the initial scene.
    /// </summary>
    internal static void Load() {
        GuiManager.Load();

        Music = new MusicResource[] {
            ResourceManager.MusicLoader.Get("music_1"),
        };

        Scene = new MainMenuScene();
    }

    /// <summary>
    /// Exectues the game's update logic. Updates the currently active scene. Is executed every frame.
    /// </summary>
    /// <param name="dT"></param>
    internal static void Update(float dT) {
        if (Music.Count > 0 && Music.All(m => !AudioManager.IsMusicPlaying(m.Key))) {
            if (WasMusicQueued)
                return;

            Random rng = new Random();
            AudioManager.PlayMusic(Music[rng.Next(Music.Count)].Key);
            WasMusicQueued = true;
        } else {
            WasMusicQueued = false;
        }

        GuiManager.Update(dT);

        lock (SceneLock) {
            // The scene is loaded in the update method to ensure scene drawing doesn't access unloaded resources.
            if (!WasSceneLoaded) {
                Scene.Load();
                WasSceneLoaded = true;
            } else
                Scene.Update(dT);
        }
    }

    internal static void RenderPostProcessed(ShaderResource shader, float dT) {
        if (!WasSceneLoaded)
            return;

        lock (SceneLock)
            Scene.RenderPostProcessed(shader, dT);
    }

    /// <summary>
    /// Draws the game. Is executed every frame.
    /// </summary>
    internal static void Render(float dT) {
        if (!WasSceneLoaded)
            return;

        lock (SceneLock)
            Scene.Render(dT);
    }

    /// <summary>
    /// Unloads the game's resources.
    /// </summary>
    internal static void Unload() {
        Scene.Unload();
        GuiManager.Unload();
    }

    internal static void SetScene(Scene scene) {
        lock (SceneLock) {
            Scene.Unload();
            GuiManager.ResetElements();
            WasSceneLoaded = false;
            Scene = scene;
        }
    }
}
