using NeuroBdayJam.App;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World.Generation;
using Raylib_CsLo;
using System.Diagnostics;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class WorldGenTestScene : Scene {

    private WorldGenerator WorldGenerator { get; set; }

    private int CurrentX;
    private int CurrentY;

    /// <summary>
    /// Called when the scene is loaded. Override this method to provide custom scene initialization logic and to load resources.
    /// </summary>
    internal override void Load() {
        RuleParser parser = new RuleParser();
        parser.Parse(
@"
1 -> CCC CCC CCC CCC
R 1
2 -> GGG GGG GGG GGG
R 2
3 -> GGG GgG GGG GGG
R 3
4 -> GGG GrG GGG GrG
R 4

5 -> CGG GgG GGC CCC
R 5
6 -> CGG GGG GGG GGC
R 6

7 -> GGG GgG GGG GgG
R 7
8 -> GrG GgG GrG GgG
R 8
9 -> GrG GGG GgG GGG
R 9
10 -> GgG GgG GGG GgG
R 10
11 -> GgG GgG GgG GgG
R 11
12 -> GgG GgG GGG GGG
R 12
13 -> GGG GgG GGG GgG
R 13

");

        WorldGenerator = new WorldGenerator(50, 50);
        WorldGenerator.SetRules(parser.Export());
        WorldGenerator.CollapseCell(0, 0, 12);
        WorldGenerator.Store(true);
        WorldGenerator.GenerateEverything(true);

        CurrentX = CurrentY = 0;

        Input.RegisterHotkey("reset_generation", KeyboardKey.KEY_R, new KeyboardKey[0]);

        Input.RegisterHotkey(GameHotkeys.MOVE_UP, KeyboardKey.KEY_W);
        Input.RegisterHotkey(GameHotkeys.MOVE_LEFT, KeyboardKey.KEY_A);
        Input.RegisterHotkey(GameHotkeys.MOVE_DOWN, KeyboardKey.KEY_S);
        Input.RegisterHotkey(GameHotkeys.MOVE_RIGHT, KeyboardKey.KEY_D);

    }

    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        Stopwatch watch = new Stopwatch();

        if (Input.IsHotkeyDown(GameHotkeys.MOVE_UP)) {
            WorldGenerator.GenerateTileRow(CurrentY + WorldGenerator.Height);
            CurrentY++;
        }
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_DOWN)) {
            WorldGenerator.GenerateTileRow(CurrentY - 1);
            CurrentY--;
        }

        if (Input.IsHotkeyDown(GameHotkeys.MOVE_LEFT)) {
            WorldGenerator.GenerateTileColumn(CurrentX + WorldGenerator.Width);
            CurrentX++;
        }
        if (Input.IsHotkeyDown(GameHotkeys.MOVE_RIGHT)) {
            WorldGenerator.GenerateTileColumn(CurrentX - 1);
            CurrentX--;
        }


        if (Input.IsHotkeyActive("reset_generation")) {
            WorldGenerator.Restore();
        }

        watch.Start();

        WorldGenerator.GenerateEverything();

        watch.Stop();

        bool shouldStore = false;

        // if (Input.IsHotkeyActive(GameHotkeys.MOVE_UP)){
        //     WorldGenerator.Translate(0, -1);
        //     shouldStore = true;
        // }
        // if (Input.IsHotkeyActive(GameHotkeys.MOVE_DOWN)){
        //     WorldGenerator.Translate(0, 1);
        //     shouldStore = true;
        // }

        // if (Input.IsHotkeyActive(GameHotkeys.MOVE_LEFT)){
        //     WorldGenerator.Translate(-1, 0);
        //     shouldStore = true;
        // }
        // if (Input.IsHotkeyActive(GameHotkeys.MOVE_RIGHT)){
        //     WorldGenerator.Translate(1, 0);
        //     shouldStore = true;
        // }

        if (shouldStore) {
            WorldGenerator.Store();
        }

        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Called every frame to draw the scene. Override this method to provide custom scene rendering logic.
    /// </summary>
    internal override void Draw(float dT) {
        WorldGenerator.DEBUG_Draw();
    }

    /// <summary>
    /// Called when the scene is about to be unloaded or replaced by another scene. Override this method to provide custom cleanup or deinitialization logic and to unload resources.
    /// </summary>
    internal override void Unload() {
        Input.UnregisterHotkey("reset_generation");
    }

}
