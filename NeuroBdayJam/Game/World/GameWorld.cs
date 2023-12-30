using NeuroBdayJam.Game.Entities;
using NeuroBdayJam.Game.Entities.Enemies;
using NeuroBdayJam.Game.Entities.Memories;
using NeuroBdayJam.Game.Memories;
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
    private HashSet<Entity> Entities { get; }
    public Memory? ActiveMemory { get; set; }
    private VedalTerminal VedalTerminal { get; set; }

    public TextureAtlas MiscAtatlas { get; private set; }
    public Tileset Tileset { get; private set; }
    private WorldTile[,] Tiles { get; }

    public GameWorld(ulong[,] tiles) {
        Tiles = CreateFromIds(tiles);
        Entities = new HashSet<Entity>();
    }

    public void Load() {
        TilesetResource tilesetResource = ResourceManager.TilesetLoader.Get("dark");
        TextureAtlasResource miscAtlasResource = ResourceManager.TextureAtlasLoader.Get("misc_atlas");
        ResourceManager.TextureAtlasLoader.Load("player_animations");

        tilesetResource.WaitForLoad();
        miscAtlasResource.WaitForLoad();

        Tileset = tilesetResource.Resource;
        MiscAtatlas = miscAtlasResource.Resource;

        Player = new Neuro(new Vector2(1.5f, 1.5f));
        AddEntity(Player);
        ActiveMemory = new Memory(new Vector2(5.5f, 5.5f), MemoryTracker.GetRandomUncollectedMemory());
        AddEntity(ActiveMemory);
        VedalTerminal = new VedalTerminal(new Vector2(1.5f, 7.5f));
        AddEntity(VedalTerminal);

        // TODO TESTING
        AddEntity(new Worm(new Vector2(5.5f, 5.5f)));
    }

    internal void Update(float dT) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tiles[x, y].Update(dT); ;
            }
        }

        foreach (Entity entity in Entities.ToList()) {
            if (entity.World == null)
                entity.Load(this);

            if (entity.IsDead) {
                entity.Unload();
                Entities.Remove(entity);
            } else
                entity.Update(dT);
        }

        Player.Update(dT);
        VedalTerminal.Update(dT);
        ActiveMemory?.Update(dT);
    }

    internal void Render(float dT) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tiles[x, y].Render(dT); ;
            }
        }

        foreach (Entity entity in Entities.ToList()) {
            if (!entity.IsDead && entity.World != null)
                entity.Render(dT);
        }

        ActiveMemory?.Render(dT);
        VedalTerminal.Render(dT);
        Player.Render(dT);
    }

    public void AddEntity(Entity entity) {
        Entities.Add(entity);
    }

    public WorldTile? GetTile(Vector2 position) {
        int x = (int)position.X;
        int y = (int)position.Y;

        return GetTile(x, y);
    }

    public WorldTile? GetTile(int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return null;

        return Tiles[x, y];
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
                WorldTile tile = Tiles[x, y];
                TileType tileType = Tileset.GetTileType(tile.Id);

                if (tileType.Collider == null)
                    continue;

                Rectangle boundsRect = new Rectangle(x + tileType.Collider.Value.x, y + tileType.Collider.Value.y, tileType.Collider.Value.width, tileType.Collider.Value.height);
                colliders.Add(boundsRect);
            }
        }

        return colliders;
    }

    private WorldTile[,] CreateFromIds(ulong[,] tiles) {
        WorldTile[,] worldTiles = new WorldTile[tiles.GetLength(0), tiles.GetLength(1)];

        for (int x = 0; x < tiles.GetLength(0); x++) {
            for (int y = 0; y < tiles.GetLength(1); y++) {
                ulong tileId = tiles[x, y];

                ulong left = x > 0 ? tiles[x - 1, y] : tileId;
                ulong right = x < tiles.GetLength(0) - 1 ? tiles[x + 1, y] : tileId;
                ulong top = y > 0 ? tiles[x, y - 1] : tileId;
                ulong bottom = y < tiles.GetLength(1) - 1 ? tiles[x, y + 1] : tileId;

                worldTiles[x, y] = new WorldTile(this, tileId, (x, y), FindConfiguration(tileId, left, right, top, bottom));
            }
        }

        return worldTiles;
    }

    private static byte FindConfiguration(ulong center, ulong left, ulong right, ulong top, ulong bottom) {
        byte configuration = 0;
        if (center == left)
            configuration = (byte)(configuration | (1 << 0));
        if (center == right)
            configuration = (byte)(configuration | (1 << 1));
        if (center == top)
            configuration = (byte)(configuration | (1 << 2));
        if (center == bottom)
            configuration = (byte)(configuration | (1 << 3));

        return configuration;
    }
}
