using NeuroBdayJam.Game.World.Generation;
using NeuroBdayJam.ResourceHandling;

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
1 -> 0 0 0 0
R 1
2 -> 1 1 1 1
R 2
3 -> 1 2 1 1
R 3
4 -> 1 3 1 3
R 4

5 -> 9 9 9 9
R 5
6 -> 9 9 9 9
R 6

7 -> 1 2 1 2
R 7
8 -> 3 2 3 2
R 8
9 -> 3 1 2 1
R 9
10 -> 2 2 1 2
R 10
11 -> 2 2 2 2
R 11
12 -> 2 2 1 1
R 12
13 -> 1 2 1 2
R 13

");

        WorldGenerator.SetRules(parser.Export());

        WorldGenerator.CollapseCell(0, 0, 5);
    }

    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        while (WorldGenerator.Step()) ;

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
    internal override void Unload() { }

}
