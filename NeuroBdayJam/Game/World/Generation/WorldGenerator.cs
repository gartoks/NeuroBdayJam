using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;
using System.Resources;
using System.Security.Cryptography.X509Certificates;

namespace NeuroBdayJam.Game.World.Generation;

internal class WorldGenerator {
    Tile[,] Tiles;
    Vector2 TileSize;

    int Width, Height;

    TextureResource[] DEBUG_Textures;

    internal WorldGenerator(int width, int height) {
        Width = width;
        Height = height;

        Tiles = new Tile[width, height];
        int minDim = Math.Min(Application.BASE_WIDTH, Application.BASE_HEIGHT);
        int maxTileDim = Math.Max(width, height);
        TileSize = new Vector2(minDim / maxTileDim);

        Reset();

        DEBUG_Textures = new TextureResource[13]{
            ResourceHandling.ResourceManager.TextureLoader.Get("0"),
            ResourceHandling.ResourceManager.TextureLoader.Get("1"),
            ResourceHandling.ResourceManager.TextureLoader.Get("2"),
            ResourceHandling.ResourceManager.TextureLoader.Get("3"),
            ResourceHandling.ResourceManager.TextureLoader.Get("4"),
            ResourceHandling.ResourceManager.TextureLoader.Get("5"),
            ResourceHandling.ResourceManager.TextureLoader.Get("6"),
            ResourceHandling.ResourceManager.TextureLoader.Get("7"),
            ResourceHandling.ResourceManager.TextureLoader.Get("8"),
            ResourceHandling.ResourceManager.TextureLoader.Get("9"),
            ResourceHandling.ResourceManager.TextureLoader.Get("10"),
            ResourceHandling.ResourceManager.TextureLoader.Get("11"),
            ResourceHandling.ResourceManager.TextureLoader.Get("12"),
        };

        for (int i=0; i<DEBUG_Textures.Length; i++){
            DEBUG_Textures[i].WaitForLoad();
        }
    }

    public void CollapseCell(int x, int y, int id) {
        Tiles[x, y].PossibleValues = (ulong)1 << (id - 1);
        Tiles[x, y].Id = id;

        if (y != 0)
            Tiles[x, y - 1].PossibleValues &= IdAndSideToPossibleNeighbours[new(id, Side.Top)];

        if (x != 0)
            Tiles[x - 1, y].PossibleValues &= IdAndSideToPossibleNeighbours[new(id, Side.Left)];

        if (y != Height - 1)
            Tiles[x, y + 1].PossibleValues &= IdAndSideToPossibleNeighbours[new(id, Side.Bottom)];

        if (x != Width - 1)
            Tiles[x + 1, y].PossibleValues &= IdAndSideToPossibleNeighbours[new(id, Side.Right)];
    }

    internal bool Step() {
        int minEntropy = int.MaxValue;
        List<(int, int)> minEntropyIndices = new();

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (Tiles[x, y].Id == 0) {
                    int thisEntropy = BitOperations.PopCount(Tiles[x, y].PossibleValues);
                    if (thisEntropy < minEntropy) {
                        minEntropy = thisEntropy;
                        minEntropyIndices.Clear();
                    }
                    if (thisEntropy == minEntropy) {
                        minEntropyIndices.Add((x, y));
                    }
                }
            }
        }

        if (minEntropyIndices.Count != 0 && minEntropy != 0) {
            (int x, int y) = minEntropyIndices[(int)((uint)Random.Shared.NextInt64() % minEntropyIndices.Count)];

            ulong possibleValuesBitmap = Tiles[x, y].PossibleValues;

            int numSkips = (int)((uint)Random.Shared.NextInt64() % BitOperations.PopCount(possibleValuesBitmap));
            for (int i = 0; i < numSkips; i++) {
                possibleValuesBitmap &= possibleValuesBitmap - 1;
            }

            CollapseCell(x, y, BitOperations.TrailingZeroCount(possibleValuesBitmap) + 1);
            return true;
        }

        return false;
    }

    public void Reset(){
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tiles[x, y] = new Tile() {
                    Pos = new Vector2(x, y),
                    Id = 0,
                    Size = TileSize,
                    PossibleValues = ulong.MaxValue
                };
            }
        }
    }

    public bool IsDone(){
        foreach (Tile tile in Tiles){
            if (tile.Id == 0){
                return false;
            }
        }
        return true;
    }


    internal void DEBUG_Draw() {
        foreach (Tile tile in Tiles) {
            tile.DEBUG_Draw(DEBUG_Textures);
        }
    }

    private struct Tile {
        public int Id;
        public Vector2 Size;
        public Vector2 Pos;

        public ulong PossibleValues;

        internal void DEBUG_Draw(TextureResource[] Textures) {
            if (Id == 0){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(255, 0, 0, 255));
            }

            else{
                int imageID = (Id-1) / 4;
                int rotation = (Id-1) % 4;

                Vector2 offset = Vector2.Zero;
                if (rotation == 0) offset = new Vector2(0);
                else if (rotation == 1) offset = new Vector2(Size.X, 0);
                else if (rotation == 2) offset = Size;
                else if (rotation == 3) offset = new Vector2(0, Size.X);


                Raylib.DrawTextureEx(Textures[imageID].Resource, Pos * Size + offset, rotation*90, Size.X / Textures[imageID].Resource.width, new Color(255, 255, 255, 255));
            }

            // Raylib.DrawTextEx(Renderer.GuiFont.Resource, "N: " + Pos.ToString(), Pos * Size, 40, 10, new Color(255, 255, 255, 255));
            // Raylib.DrawTextEx(Renderer.GuiFont.Resource, "ID: " + Id.ToString(), Pos * Size + new Vector2(0, 50), 40, 10, new Color(255, 255, 255, 255));
        }
    }

    public void SetRules(Dictionary<(int, Side), ulong> rules){
        IdAndSideToPossibleNeighbours = rules;
    }

    public enum Side{
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }

    Dictionary<(int, Side), ulong> IdAndSideToPossibleNeighbours = new();
}