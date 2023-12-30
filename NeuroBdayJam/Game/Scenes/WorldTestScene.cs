using NeuroBdayJam.App;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Game.World.Generation;
using Raylib_CsLo;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class WorldTestScene : Scene {

    private GameWorld World { get; set; }
    private WorldGenerator WorldGenerator { get; set; }
    private Dictionary<ulong, ulong> ExportSettings { get; set; }

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
        Input.RegisterHotkey(GameHotkeys.USE_MEMORY_1, KeyboardKey.KEY_ONE);
        Input.RegisterHotkey(GameHotkeys.USE_MEMORY_2, KeyboardKey.KEY_TWO);
        Input.RegisterHotkey(GameHotkeys.USE_MEMORY_3, KeyboardKey.KEY_THREE);
        Input.RegisterHotkey("DEBUG_reset_generation", KeyboardKey.KEY_R, new KeyboardKey[0]);

        World = CreateTestWorld("Map_Test_1");
        //World = new GameWorld(WorldGenerator.ExportToUlongs(ExportSettings));
        World.Load();
    }

    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        World.Update(dT);

        if (Input.IsHotkeyActive("DEBUG_reset_generation")) {
            WorldGenerator.Restore();
            WorldGenerator.GenerateEverything();
            World = new GameWorld(WorldGenerator.ExportToUlongs());
            World.Load();
        }
    }

    /// <summary>
    /// Called every frame to draw the scene. Override this method to provide custom scene rendering logic.
    /// </summary>
    internal override void Draw(float dT) {
        World.Render(dT);
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
            string line = lines[y];

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
