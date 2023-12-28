﻿using NeuroBdayJam.Game.World;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class WorldTestScene : Scene {

    private GameWorld World { get; set; }

    /// <summary>
    /// Called when the scene is loaded. Override this method to provide custom scene initialization logic and to load resources.
    /// </summary>
    internal override void Load() {
        World = LoadTestWorld("Map_Test_0");
    }

    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        World.Update(dT);
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

    private static GameWorld LoadTestWorld(string fileName) {
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
                        tiles[x, y] = 0;
                        break;
                    case 'x':
                        tiles[x, y] = 1;
                        break;
                    default:
                        break;
                }

            }
        }

        return new GameWorld(new Tileset(), tiles);
    }
}