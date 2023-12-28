namespace NeuroBdayJam.Game.World;
internal class GameWorld {
    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private Tileset Tileset { get; set; }
    private ulong[,] Tiles { get; }

    public GameWorld(Tileset tileset, ulong[,] tiles) {
        Tileset = tileset;
        Tiles = tiles;

    }

    internal void Update(float dT) {
    }

    internal void Render(float dT) {
    }
}
