using System.Numerics;
using Raylib_CsLo;

namespace NeuroBdayJam.WorldGen;
/// <summary>
/// Class for one set of game resources. Doesn't cache anything.
/// </summary>

internal class World{
    Tile[,] Tiles;
    Vector2 TileSize;

    internal World(int width, int height){
        Tiles = new Tile[width, height];
        int minDim = Math.Min(Application.BASE_WIDTH, Application.BASE_HEIGHT);
        int maxTileDim = Math.Max(width, height);
        TileSize = new Vector2(minDim/maxTileDim);

        int i=0;
        for (int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                Tiles[x, y] = new Tile(){
                    DEBUG_color = new Color((byte)(Random.Shared.NextSingle()*255), (byte)(Random.Shared.NextSingle()*255), (byte)(Random.Shared.NextSingle()*255), (byte)255),
                    Pos = new Vector2(x, y),
                    Id = i++,
                    Size = TileSize
                };
            }
        }
    }

    internal void DEBUG_Draw(){
        foreach (Tile tile in Tiles){
            tile.DEBUG_Draw();
        }
    }
    
    internal struct Tile{
        public int Id;
        public Vector2 Size;
        public Vector2 Pos;

        public Color DEBUG_color;

        internal void DEBUG_Draw(){
            Raylib.DrawRectangleV(Pos * Size, Size, DEBUG_color);
        }
        
    }
}