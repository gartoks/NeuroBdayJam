using NeuroBdayJam.App;
using NeuroBdayJam.Game.World.Generation;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class WorldGenTestScene : Scene {

    private WorldGenerator WorldGenerator { get; set; }

    /// <summary>
    /// Called when the scene is loaded. Override this method to provide custom scene initialization logic and to load resources.
    /// </summary>
    internal override void Load() {
        WorldGenerator = new WorldGenerator(20, 20);
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

        WorldGenerator.SetRules(parser.Export());

        Input.RegisterHotkey("reset_generation", Raylib_CsLo.KeyboardKey.KEY_R, new Raylib_CsLo.KeyboardKey[0]);
    }

    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        if (Input.IsHotkeyActive("reset_generation")){
            WorldGenerator.Reset();
            WorldGenerator.CollapseCell(0, 0, 5); 
        }

        watch.Start();

        while (!WorldGenerator.IsDone()){
            WorldGenerator.Reset();
            WorldGenerator.CollapseCell(0, 0, 5); 
            while (WorldGenerator.Step()) ;
        }


        watch.Stop();

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
