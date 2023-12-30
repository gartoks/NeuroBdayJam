﻿using NeuroBdayJam.App;
using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.Game.World.Generation;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Scenes;
/// <summary>
/// Represents a base class for game scenes. Provides methods for scene lifecycle including loading, updating, drawing, and unloading.
/// </summary>
internal class GameplayTestScene : Scene {

    private GameWorld World { get; set; }
    private WorldGenerator WorldGenerator { get; set; }
    private Dictionary<ulong, ulong> ExportSettings { get; set; }

    private GuiDynamicLabel MemoryTrackerLabel;

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

        WorldGenerator = new WorldGenerator(15, 10);
        RuleParser parser = new();
        parser.Parse(
@"
2 -> WFW WWW WFW WWW
R 2
4 -> WWW WWW WWW WWW
R 4
R 4
R 4
R 4
5 -> WFW WFW WWW WWW
R 5
6 -> WFW WFW WFW WWW
R 6
7 -> WFW WWW WWW WWW
R 7
"
        );

        WorldGenerator.SetRules(parser.Export());
        WorldGenerator.CollapseCell(0, 0, 2);
        WorldGenerator.Store();
        WorldGenerator.GenerateEverything();

        int id = 0;
        ExportSettings = new Dictionary<ulong, ulong>{
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},

            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},

            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},
            {(ulong)1 << id++, 2},

            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},

            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
            {(ulong)1 << id++, 1},
        };

        World = new GameWorld(WorldGenerator.ExportToUlongs(ExportSettings));
        World = CreateTestWorld("Map_Test_1");
        World.Load();

        MemoryTrackerLabel = new(0, Application.BASE_HEIGHT - 50, "0/3", 50, new Vector2(0, 1));
    }

    /// <summary>
    /// Called every frame to update the scene's state. 
    /// </summary>
    /// <param name="dT">The delta time since the last frame, typically used for frame-rate independent updates.</param>
    internal override void Update(float dT) {
        World.Update(dT);
        MemoryTrackerLabel.Text = $"{MemoryTracker.NumTemproaryMemories} + {MemoryTracker.NumMemoriesCollected}/{MemoryTracker.NumMemories}";
    }

    /// <summary>
    /// Called every frame to draw the scene. Override this method to provide custom scene rendering logic.
    /// </summary>
    internal override void Draw(float dT) {
        World.Render(dT);
        MemoryTrackerLabel.Draw();
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
