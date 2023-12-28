using NeuroBdayJam.Game.WorldGeneration;

namespace NeuroBdayJam.WorldGen;
/// <summary>
/// Class for one set of game resources. Doesn't cache anything.
/// </summary>

internal class World {
    private const float TILE_SIZE = 32;

    public int TileWidth => Tiles.GetLength(0);
    public int TileHeight => Tiles.GetLength(1);

    private ulong[,] Tiles { get; }

    internal World(Tileset tileset, ulong[,] tiles) {
        Tiles = tiles;
    }

    internal void DEBUG_Draw() {
        foreach (ulong tileId in Tiles) {
            //tile.DEBUG_Draw();
        }
    }

    /*internal struct Tile {
        public int Id;
        public Vector2 Size;
        public Vector2 Pos;

        public Color DEBUG_color;

        internal void DEBUG_Draw() {
            Raylib.DrawRectangleV(Pos * Size, Size, DEBUG_color);
        }

    }*/
}