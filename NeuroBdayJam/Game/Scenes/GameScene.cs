using NeuroBdayJam.App;
using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Graphics;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class GameScene : Scene {
    private FrameAnimator LoadingIndicatorAnimator { get; set; }
    private TextureAtlasResource AnimationsTextureAtlas { get; set; }
    private GuiDynamicLabel MemoryTrackerLabel { get; set; }

    private GameWorld World { get; set; }
    private bool IsWorldLoaded { get; set; }
    private bool IsLoadingTextureLoaded { get; set; }

    /// <summary>
    /// Called when the scene is loaded. Override this method to provide custom scene initialization logic and to load resources.
    /// </summary>
    internal override void Load() {
        Input.RegisterHotkey(GameHotkeys.MOVE_UP, KeyboardKey.KEY_W);
        Input.RegisterHotkey(GameHotkeys.MOVE_LEFT, KeyboardKey.KEY_A);
        Input.RegisterHotkey(GameHotkeys.MOVE_DOWN, KeyboardKey.KEY_S);
        Input.RegisterHotkey(GameHotkeys.MOVE_RIGHT, KeyboardKey.KEY_D);
        Input.RegisterHotkey(GameHotkeys.SNEAK, KeyboardKey.KEY_LEFT_CONTROL);
        Input.RegisterHotkey(GameHotkeys.SPRINT, KeyboardKey.KEY_LEFT_SHIFT);
        Input.RegisterHotkey(GameHotkeys.INTERACT, KeyboardKey.KEY_E);
        Input.RegisterHotkey(GameHotkeys.USE_MEMORY_1, KeyboardKey.KEY_ONE);
        Input.RegisterHotkey(GameHotkeys.USE_MEMORY_2, KeyboardKey.KEY_TWO);
        Input.RegisterHotkey(GameHotkeys.USE_MEMORY_3, KeyboardKey.KEY_THREE);


        // World = CreateTestWorld("Map_Test_1");
        IsWorldLoaded = false;
        Task.Run(() => {
            World = new GameWorld();
            World.Load();
            IsWorldLoaded = true;
        });

        MemoryTrackerLabel = new(0, Application.BASE_HEIGHT - 50, "0/3", 50, new Vector2(0, 1));

        AnimationsTextureAtlas = ResourceManager.TextureAtlasLoader.Get("player_animations");
        LoadingIndicatorAnimator = new FrameAnimator(1f / 12f);
        IsLoadingTextureLoaded = false;
    }



    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        if (!IsLoadingTextureLoaded) {
            AnimationsTextureAtlas.WaitForLoad();

            for (int i = 0; i < 8; i++)
                LoadingIndicatorAnimator.AddFrameKey($"walk_{i}", AnimationsTextureAtlas.Resource.GetSubTexture($"walk_right_{i}")!);
            LoadingIndicatorAnimator.AddFrameSequence("walk", 1, "walk_0", "walk_0", "walk_1", "walk_1", "walk_2", "walk_2", "walk_3", "walk_3", "walk_4", "walk_4", "walk_5", "walk_5", "walk_6", "walk_6", "walk_7", "walk_7");
            LoadingIndicatorAnimator.SetDefaultSequence("walk");
            LoadingIndicatorAnimator.StartSequence("walk");
            IsLoadingTextureLoaded = true;
        }

        if (IsWorldLoaded) {
            World.Update(dT);
            MemoryTrackerLabel.Text = $"Memories: {World.MemoryTracker.NumMemoriesCollected}/{World.MemoryTracker.NumMemories}";
        }
    }

    /// <summary>
    /// Called every frame to draw the scene. Override this method to provide custom scene rendering logic.
    /// </summary>
    internal override void Render(float dT) {
        if (IsWorldLoaded) {
            World.Render(dT);
            MemoryTrackerLabel.Draw();
        } else if (IsLoadingTextureLoaded) {
            if (LoadingIndicatorAnimator.IsReady) {
                LoadingIndicatorAnimator.StartSequence("walk");
            }
            LoadingIndicatorAnimator.Render(dT, new Rectangle(Application.BASE_WIDTH / 2, Application.BASE_HEIGHT / 2, Application.BASE_HEIGHT * 0.2f, Application.BASE_HEIGHT * 0.2f), 0, new Vector2(0.5f, 0.5f), Raylib.WHITE);
        }
    }

    internal override void RenderPostProcessed(ShaderResource shader, float dT) {
        if (!IsWorldLoaded)
            return;

        World.RenderPostProcessed(shader, dT);
    }

    /// <summary>
    /// Called when the scene is about to be unloaded or replaced by another scene. Override this method to provide custom cleanup or deinitialization logic and to unload resources.
    /// </summary>
    internal override void Unload() { }

    private static GameWorld CreateTestWorld(string fileName) {
        string path = Path.Combine("Resources", "TestStuff", "Maps", $"{fileName}.txt");
        string[] lines = File.ReadAllLines(path);

        int width = lines[0].Length;
        int height = lines.Length;

        ulong[,] tiles = new ulong[width, height];
        for (int y = 0; y < lines.Length; y++) {
            string line = lines[y].Trim();

            for (int x = 0; x < line.Length; x++) {
                char tileChar = line[x];

                switch (tileChar) {
                    case ' ':
                        tiles[x, y] = 1;
                        break;
                    case 'x':
                        tiles[x, y] = 2;
                        break;
                    default:
                        break;
                }

            }
        }

        return new GameWorld(tiles);
    }
}
