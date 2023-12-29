using NeuroBdayJam.Game.Entities;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.World;
internal class GameWorld {
    public const float TILE_SIZE = 64;

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    public Neuro Player { get; set; }
    private Worm Worm { get; set; }

    private Tileset Tileset { get; set; }
    private ulong[,] Tiles { get; }

    public GameWorld(ulong[,] tiles) {
        Tiles = tiles;

    }

    public void Load() {
        TilesetResource tilesetResource = ResourceManager.TilesetLoader.Get("dark");
        ResourceManager.TextureLoader.Load("player");


        tilesetResource.WaitForLoad();


        Tileset = tilesetResource.Resource;
        Player = new Neuro(this, new Vector2(1.5f, 1.5f));
        Worm = new Worm(this, new Vector2(5.5f, 5.5f));
    }

    internal void Update(float dT) {
        Player.Update(dT);
        Worm.Update(dT);
    }

    internal void Render(float dT) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                DrawTile(x, y);
            }
        }

        Worm.Render(dT);
        Player.Render(dT);
    }

    public IReadOnlyList<Rectangle> GetSurroundingTileColliders(Vector2 position) {
        int pX = (int)position.X;
        int pY = (int)position.Y;

        int minX = Math.Max(0, pX - 1);
        int minY = Math.Max(0, pY - 1);
        int maxX = Math.Min(Width - 1, pX + 1);
        int maxY = Math.Min(Height - 1, pY + 1);

        List<Rectangle> colliders = new();
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                ulong tileId = Tiles[x, y];
                TileType tileType = Tileset.GetTileType(tileId);

                if (tileType.Collider == null)
                    continue;

                Rectangle boundsRect = new Rectangle(x + tileType.Collider.Value.x, y + tileType.Collider.Value.y, tileType.Collider.Value.width, tileType.Collider.Value.height);
                colliders.Add(boundsRect);
            }
        }

        return colliders;
    }

    private void DrawTile(int x, int y) {
        ulong tileId = Tiles[x, y];
        TileType tileType = Tileset.GetTileType(tileId);

        SubTexture texture = Tileset.GetTileTexture(tileId);
        texture.Draw(new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE), new Vector2(0, 0));

        if (Application.DRAW_DEBUG && tileType.Collider != null) {
            Rectangle boundsRect = new Rectangle((x + tileType.Collider.Value.x) * TILE_SIZE, (y + tileType.Collider.Value.y) * TILE_SIZE, tileType.Collider.Value.width * TILE_SIZE, tileType.Collider.Value.height * TILE_SIZE);
            Raylib.DrawRectangleLinesEx(boundsRect, 1, Raylib.LIME);
        }
    }
}
