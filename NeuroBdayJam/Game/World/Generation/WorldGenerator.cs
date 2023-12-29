using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using System.Numerics;

namespace NeuroBdayJam.Game.World.Generation;

internal class WorldGenerator {
    Tile[,] Tiles;
    Vector2 TileSize;

    int Width, Height;

    internal WorldGenerator(int width, int height) {
        Width = width;
        Height = height;

        Tiles = new Tile[width, height];
        int minDim = Math.Min(Application.BASE_WIDTH, Application.BASE_HEIGHT);
        int maxTileDim = Math.Max(width, height);
        TileSize = new Vector2(minDim / maxTileDim);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Tiles[x, y] = new Tile() {
                    Pos = new Vector2(x, y),
                    Id = 0,
                    Size = TileSize,
                    PossibleValues = ulong.MaxValue
                };
            }
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

    internal void DEBUG_Draw() {
        foreach (Tile tile in Tiles) {
            tile.DEBUG_Draw();
        }
    }

    private struct Tile {
        public int Id;
        public Vector2 Size;
        public Vector2 Pos;

        public ulong PossibleValues;

        internal void DEBUG_Draw() {
            if (Id == 0){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(255, 0, 0, 255));
            }
            else if (Id == 1 || Id == 3){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(0, 0, 255, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0, 0.25f), Size / new Vector2(1, 2), new Color(100, 100, 100, 255));
            }
            else if (Id == 2 || Id == 4){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(0, 0, 255, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0), Size / new Vector2(2, 1), new Color(100, 100, 100, 255));
            }
            else if (Id == 5){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(0, 0, 255, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0f, 0.25f), Size * new Vector2(0.75f, 0.5f), new Color(100, 100, 100, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0f), Size * new Vector2(0.5f, 0.75f), new Color(100, 100, 100, 255));
            }
            else if (Id == 6){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(0, 0, 255, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0.25f), Size * new Vector2(0.75f, 0.5f), new Color(100, 100, 100, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0f), Size * new Vector2(0.5f, 0.75f), new Color(100, 100, 100, 255));
            }
            else if (Id == 7){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(0, 0, 255, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0.25f), Size * new Vector2(0.5f, 0.75f), new Color(100, 100, 100, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0.25f), Size * new Vector2(0.75f, 0.5f), new Color(100, 100, 100, 255));
            }
            else if (Id == 8){
                Raylib.DrawRectangleV(Pos * Size, Size, new Color(0, 0, 255, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0f, 0.25f), Size * new Vector2(0.75f, 0.5f), new Color(100, 100, 100, 255));
                Raylib.DrawRectangleV(Pos * Size + Size * new Vector2(0.25f, 0.25f), Size * new Vector2(0.5f, 0.75f), new Color(100, 100, 100, 255));
            }
            else
                return;


            // Raylib.DrawTextEx(Renderer.GuiFont.Resource, "N: " + PossibleValues.Count.ToString(), Pos * Size, 40, 10, new Color(255, 255, 255, 255));
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