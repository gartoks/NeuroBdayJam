using NeuroBdayJam.App;
using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Graphics;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.ComponentModel;
using System.Numerics;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class GameScene : Scene {
    private FrameAnimator LoadingIndicatorAnimator { get; set; }
    private TextureAtlasResource AnimationsTextureAtlas { get; set; }
    private GuiDynamicLabel MemoryTrackerLabel { get; set; }

    private GuiLabel PauseMenuText { get; set; }
    private GuiPanel PauseMenuPanel { get; set; }
    private GuiTextButton PauseMenuContinueButton { get; set; }
    private GuiTextButton PauseMenuQuitButton { get; set; }

    private GuiLabel ConfirmMenuText { get; set; }
    private GuiPanel ConfirmMenuPanel { get; set; }
    private GuiTextButton ConfirmMenuQuitButton { get; set; }
    private GuiTextButton ConfirmMenuCancelButton { get; set; }

    private GameWorld World { get; set; }
    private bool IsWorldLoaded { get; set; }
    private bool IsLoadingTextureLoaded { get; set; }
    private bool IsPaused { get; set; }
    private bool IsQuitting { get; set; }
    private float StoredTimeScale { get; set; }

    private List<(string memoryName, GuiTexturePanel panel)> AbilityPanels;

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
        Input.RegisterHotkey(GameHotkeys.PAUSE, KeyboardKey.KEY_ESCAPE);


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

        PauseMenuPanel = new GuiPanel("0.5 0.7 0.3 0.3", "panel", new Vector2(0.25f, 0.5f));
        PauseMenuText = new GuiLabel("0.5 0.48 0.27 0.125", "Paused", new Vector2(0.5f, 0.5f));
        PauseMenuContinueButton = new GuiTextButton("0.5 0.56 0.27 0.0625", "Continue", new Vector2(0.5f, 0.5f));
        PauseMenuQuitButton = new GuiTextButton("0.5 0.64 0.27 0.0625", "Quit", new Vector2(0.5f, 0.5f));

        ConfirmMenuPanel = new GuiPanel("0.5 0.7 0.3 0.3", "panel", new Vector2(0.25f, 0.5f));
        ConfirmMenuText = new GuiLabel("0.5 0.48 0.27 0.0625", "Loss of X memories. Continue?", new Vector2(0.5f, 0.5f));
        ConfirmMenuQuitButton = new GuiTextButton("0.5 0.64 0.27 0.0625", "Quit", new Vector2(0.5f, 0.5f));
        ConfirmMenuCancelButton = new GuiTextButton("0.5 0.56 0.27 0.0625", "Cancel", new Vector2(0.5f, 0.5f));

        AbilityPanels = new(){
            new("Memory 1", new GuiTexturePanel("0.08 0.95 90px 90px", "camoflauge", new Vector2(0.5f, 0.5f))),
            new("Memory 2", new GuiTexturePanel("0.14 0.95 90px 90px", "dash", new Vector2(0.5f, 0.5f))),
            new("Memory 3", new GuiTexturePanel("0.20 0.95 90px 90px", "dash", new Vector2(0.5f, 0.5f)))
        };

        foreach((string _, GuiTexturePanel panel) in AbilityPanels){
            panel.TextureScale = new Vector2(0.8f);
            panel.Load();
        }
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

            DrawAbilityPanels();

            DrawPauseMenu();
            DrawConfirmMenu();

        } else if (IsLoadingTextureLoaded) {
            if (LoadingIndicatorAnimator.IsReady) {
                LoadingIndicatorAnimator.StartSequence("walk");
            }
            LoadingIndicatorAnimator.Render(dT, new Rectangle(Application.BASE_WIDTH / 2, Application.BASE_HEIGHT / 2, Application.BASE_HEIGHT * 0.2f, Application.BASE_HEIGHT * 0.2f), 0, new Vector2(0.5f, 0.5f), Raylib.WHITE);
        }
    }

    internal void DrawPauseMenu(){
        if (IsPaused && !IsQuitting){
            PauseMenuPanel.Draw();
            PauseMenuText.Draw();
            PauseMenuContinueButton.Draw();
            PauseMenuQuitButton.Draw();
        }

        if (Input.IsHotkeyActive(GameHotkeys.PAUSE) || (IsPaused && PauseMenuContinueButton.IsClicked)){
            if (IsPaused){
                World.TimeScale = StoredTimeScale;
            }
            else{
                StoredTimeScale = World.TimeScale;
                World.TimeScale = 0;
            }
            IsPaused = !IsPaused;
        }

        if (IsPaused && PauseMenuQuitButton.IsClicked){
            if (World.MemoryTracker.NumTemproaryMemories > 0){
                ConfirmMenuText.Text = $"You'll loose {World.MemoryTracker.NumTemproaryMemories} memor{(World.MemoryTracker.NumTemproaryMemories > 1 ? "ies" : "y")}. Continue?";
                IsQuitting = true;
            }
            else{
                GameManager.SetScene(new MainMenuScene());
            }
        }
    }

    internal void DrawConfirmMenu(){
        if (IsQuitting){
            ConfirmMenuPanel.Draw();
            ConfirmMenuText.Draw();
            ConfirmMenuCancelButton.Draw();
            ConfirmMenuQuitButton.Draw();

            if (ConfirmMenuQuitButton.IsClicked){
                IsQuitting = false;
                GameManager.SetScene(new MainMenuScene());
            }
            if (ConfirmMenuCancelButton.IsClicked){
                IsQuitting = false;
            }
        }

    }

    internal void DrawAbilityPanels(){
        foreach((string name, GuiTexturePanel panel) in AbilityPanels){
            if (World.MemoryTracker.IsMemoryCollected(name))
                panel.Draw();
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
    internal override void Unload() {
        Input.UnregisterHotkey(GameHotkeys.MOVE_UP);
        Input.UnregisterHotkey(GameHotkeys.MOVE_LEFT);
        Input.UnregisterHotkey(GameHotkeys.MOVE_DOWN);
        Input.UnregisterHotkey(GameHotkeys.MOVE_RIGHT);
        Input.UnregisterHotkey(GameHotkeys.SNEAK);
        Input.UnregisterHotkey(GameHotkeys.SPRINT);
        Input.UnregisterHotkey(GameHotkeys.INTERACT);
        Input.UnregisterHotkey(GameHotkeys.USE_MEMORY_1);
        Input.UnregisterHotkey(GameHotkeys.USE_MEMORY_2);
        Input.UnregisterHotkey(GameHotkeys.USE_MEMORY_3);
        Input.UnregisterHotkey(GameHotkeys.PAUSE);

        World.Unload();
    }

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
