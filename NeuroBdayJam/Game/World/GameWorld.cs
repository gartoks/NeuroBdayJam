using NeuroBdayJam.Game.Entities;
using NeuroBdayJam.Game.Entities.Enemies;
using NeuroBdayJam.Game.Entities.Memories;
using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.World.Generation;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.World;
internal class GameWorld {
    public const float TILE_SIZE = 64;

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private WorldGenerator? WorldGenerator { get; }
    private WorldTile[,] Tiles { get; }

    public Neuro Player { get; set; }
    private HashSet<Entity> Entities { get; }
    public Memory? ActiveMemory { get; set; }

    public Tileset Tileset { get; private set; }
    public TextureAtlas MiscAtlas { get; private set; }

    private Vector2 TopLeftCorner { get; set; }
    private Vector2 CenterOfScreen => TopLeftCorner + new Vector2(Width, Height) / 2.0f;
    private Vector2 LastWorldgenTLCorner { get; set; }
    private Vector2 WorldgenDelta => LastWorldgenTLCorner - TopLeftCorner;

    public delegate bool SpawnCondition(WorldTile tile);
    private List<(Type entity, float spawnRate, SpawnCondition condition)> Spawnables { get; }

    public GameWorld() {
        float visibleTileWidth = Application.BASE_WIDTH / TILE_SIZE;
        float visibleTileHeight = Application.BASE_HEIGHT / TILE_SIZE;
        int tileWidth = (int)Math.Ceiling(visibleTileWidth + 6);
        int tileHeight = (int)Math.Ceiling(visibleTileHeight + 6);
        WorldGenerator = new DefaultWorldGenerator(tileWidth, tileHeight);

        Tiles = new WorldTile[tileWidth, tileHeight];
        Entities = new HashSet<Entity>();
        TopLeftCorner = new Vector2(0, 0);
        LastWorldgenTLCorner = TopLeftCorner;

        Spawnables = new(){
            new(typeof(Memory), 0.0015f, (tile) => MemoryTracker.NumUncollectedMemories > 0 && ActiveMemory == null),
            new(typeof(VedalTerminal), 0.003f, (_) => true),
            new(typeof(Worm), 0.005f, (_) => true),
        };
    }
    public GameWorld(ulong[,] tiles) {
        Tiles = new WorldTile[tiles.GetLength(0), tiles.GetLength(1)];
        SetFromIds(Tiles, tiles, 0, 0);

        Entities = new HashSet<Entity>();
        TopLeftCorner = new Vector2(0, 0);
        LastWorldgenTLCorner = TopLeftCorner;
    }

    public void Load() {
        if (WorldGenerator != null)
            SetFromIds(Tiles, WorldGenerator.ExportToUlongs(), 0, 0);

        TilesetResource tilesetResource = ResourceManager.TilesetLoader.Get("dark");
        TextureAtlasResource miscAtlasResource = ResourceManager.TextureAtlasLoader.Get("misc_atlas");
        ResourceManager.TextureAtlasLoader.Load("player_animations");

        tilesetResource.WaitForLoad();
        miscAtlasResource.WaitForLoad();

        Tileset = tilesetResource.Resource;
        MiscAtlas = miscAtlasResource.Resource;

        Player = new Neuro(new Vector2(Width / 2, Height / 2));
        AddEntity(Player);

        ActiveMemory = new Memory(new Vector2(5.5f, 5.5f));
        AddEntity(ActiveMemory);

        AddEntity(new VedalTerminal(new Vector2(1.5f, 7.5f)));

        // TODO TESTING
        AddEntity(new Worm(new Vector2(5.5f, 5.5f)));
    }

    internal void Update(float dT) {
        TopLeftCorner = Player.Position - new Vector2(Width, Height) / 2.0f;
        //if ((Player.Position - CenterOfScreen).Length() > Math.Min(Width, Height) / 5) {
        //    Vector2 delta = Player.Position - CenterOfScreen;
        //    delta *= dT;

        //    TopLeftCorner += Vector2.Normalize(delta) * delta.LengthSquared() * 8;
        //}

        if (WorldgenDelta.X > 1) {
            MoveWorldHorizontally(-1);
        }
        if (WorldgenDelta.X < -1) {
            MoveWorldHorizontally(1);
        }
        if (WorldgenDelta.Y > 1) {
            MoveWorldVertically(-1);
        }
        if (WorldgenDelta.Y < -1) {
            MoveWorldVertically(1);
        }

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tiles[x, y].Update(dT);
            }
        }

        if (ActiveMemory != null && ActiveMemory.IsDead)
            ActiveMemory = null;

        foreach (Entity entity in Entities.ToList()) {
            if (entity.World == null)
                entity.Load(this);

            if (entity.IsDead) {
                entity.Unload();
                Entities.Remove(entity);
            } else if (entity.Position.X < TopLeftCorner.X || entity.Position.X > TopLeftCorner.X + Width || entity.Position.Y < TopLeftCorner.Y || entity.Position.Y > TopLeftCorner.Y + Height) {
                if (entity is not Neuro) {
                    entity.Unload();
                    Entities.Remove(entity);
                }
            } else
                entity.Update(dT);
        }

        //Player.Update(dT);
    }

    internal void Render(float dT) {
        RlGl.rlPushMatrix();


        float visibleTileWidth = Application.BASE_WIDTH / TILE_SIZE;
        float visibleTileHeight = Application.BASE_HEIGHT / TILE_SIZE;
        int xOffset = ((int)(Width - visibleTileWidth) / 2);
        int yOffset = ((int)(Height - visibleTileHeight) / 2);

        RlGl.rlTranslatef(-(TopLeftCorner.X + xOffset) * TILE_SIZE, -(TopLeftCorner.Y + yOffset) * TILE_SIZE, 0);
        //RlGl.rlTranslatef(-TopLeftCorner.X * TILE_SIZE, -TopLeftCorner.Y * TILE_SIZE, 0);

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tiles[x, y].Render(dT);
            }
        }

        foreach (Entity entity in Entities.ToList()) {
            if (!entity.IsDead && entity.World != null)
                entity.Render(dT);
        }

        //ActiveMemory?.Render(dT);
        //Player.Render(dT);

        if (Application.DRAW_DEBUG) {
            Raylib.DrawCircleV(CenterOfScreen * TILE_SIZE, 10, Raylib.RED);
            Raylib.DrawCircleLines((int)(CenterOfScreen.X * TILE_SIZE), (int)(CenterOfScreen.Y * TILE_SIZE), Math.Min(Width, Height) / 5 * TILE_SIZE, Raylib.RED);
        }
        RlGl.rlPopMatrix();
        if (Application.DRAW_DEBUG) {
            Raylib.DrawCircleV(-TopLeftCorner * TILE_SIZE, 10, Raylib.RED);
            Raylib.DrawCircleV(-LastWorldgenTLCorner * TILE_SIZE, 10, Raylib.YELLOW);

        }
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
        position = WorldToTileIndexSpace(position);

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

                Vector2 worldPos = TileIndexToWorldSpace(new(x, y));

                Rectangle boundsRect = new Rectangle(worldPos.X + tileType.Collider.Value.x, worldPos.Y + tileType.Collider.Value.y, tileType.Collider.Value.width, tileType.Collider.Value.height);
                colliders.Add(boundsRect);
            }
        }

        return colliders;
    }

    private void MoveWorldHorizontally(int dx) {
        if (WorldGenerator == null)
            return;

        LastWorldgenTLCorner += new Vector2(dx, 0);

        bool isPositive = dx > 0;
        int absDX = Math.Abs(dx);

        {
            int startX = isPositive ? 0 : Width - 1;
            int endX = isPositive ? Width - absDX : absDX - 1;

            for (int x = startX; x != endX; x += Math.Sign(dx)) {
                for (int y = 0; y < Height; y++) {
                    Tiles[x, y] = Tiles[x + dx, y];
                }
            }
        }

        WorldGenerator.Translate(-dx, 0);
        WorldTile[,] newTiles = CreateFromIds(WorldGenerator.ExportToUlongs(), (int)LastWorldgenTLCorner.X, (int)LastWorldgenTLCorner.Y);

        {
            // copies one extra column to update the texture
            int startX = isPositive ? Width - absDX - 1 : 0;
            int endX = isPositive ? Width : absDX + 1;

            for (int x = startX; x < endX; x++) {
                for (int y = 0; y < Height; y++) {
                    Tiles[x, y] = newTiles[x, y];
                    if (x != (isPositive ? startX : endX - 1)) {
                        OnTileGenerate(Tiles[x, y]);
                    }
                }
            }
        }
    }

    private void MoveWorldVertically(int dy) {
        if (WorldGenerator == null)
            return;

        LastWorldgenTLCorner += new Vector2(0, dy);

        bool isPositive = dy > 0;
        int absDY = Math.Abs(dy);

        {
            int startY = isPositive ? 0 : Height - 1;
            int endY = isPositive ? Height - absDY : absDY - 1;

            for (int x = 0; x < Width; x++) {
                for (int y = startY; y != endY; y += Math.Sign(dy)) {
                    Tiles[x, y] = Tiles[x, y + dy];
                }
            }
        }
        WorldGenerator.Translate(0, -dy);
        WorldTile[,] newTiles = CreateFromIds(WorldGenerator.ExportToUlongs(), (int)LastWorldgenTLCorner.X, (int)LastWorldgenTLCorner.Y);

        {
            // copies one extra row to update the texture
            int startY = isPositive ? Height - absDY - 1 : 0;
            int endY = isPositive ? Height : absDY + 1;

            for (int x = 0; x < Width; x++) {
                for (int y = startY; y < endY; y++) {
                    Tiles[x, y] = newTiles[x, y];
                    if (y != (isPositive ? startY : endY - 1)) {
                        OnTileGenerate(Tiles[x, y]);
                    }
                }
            }
        }
    }

    private void OnTileGenerate(WorldTile tile) {
        TileType tileType = Tileset.GetTileType(tile.Id);
        if (tileType.Collider == null) {
            // floor
            foreach ((Type entityType, float spawnRate, SpawnCondition condition) in Spawnables.Shuffle(Random.Shared)) {
                if (Random.Shared.NextSingle() <= spawnRate) {
                    if (!condition(tile))
                        continue;

                    Entity? newEntity = (Entity)Activator.CreateInstance(entityType, new Vector2(tile.Position.x + 0.5f, tile.Position.y + 0.5f));
                    AddEntity(newEntity);

                    if (entityType == typeof(Memory)) {
                        ActiveMemory = (Memory)newEntity;
                    }
                    break;
                }
            }
        }
    }

    private WorldTile[,] CreateFromIds(ulong[,] tileIds, int xOffset, int yOffset) {
        WorldTile[,] newTiles = new WorldTile[tileIds.GetLength(0), tileIds.GetLength(1)];
        SetFromIds(newTiles, tileIds, xOffset, yOffset);
        return newTiles;
    }

    private void SetFromIds(WorldTile[,] tiles, ulong[,] tileIds, int xOffset, int yOffset) {
        for (int x = 0; x < tileIds.GetLength(0); x++) {
            for (int y = 0; y < tileIds.GetLength(1); y++) {
                ulong tileId = tileIds[x, y];

                ulong left = x > 0 ? tileIds[x - 1, y] : tileId;
                ulong right = x < tileIds.GetLength(0) - 1 ? tileIds[x + 1, y] : tileId;
                ulong top = y > 0 ? tileIds[x, y - 1] : tileId;
                ulong bottom = y < tileIds.GetLength(1) - 1 ? tileIds[x, y + 1] : tileId;

                tiles[x, y] = new WorldTile(this, tileId, (x + xOffset, y + yOffset), FindConfiguration(tileId, left, right, top, bottom));
            }
        }
    }

    public Vector2 WorldToTileIndexSpace(Vector2 vec) {
        return vec - LastWorldgenTLCorner;
    }
    public Vector2 TileIndexToWorldSpace(Vector2 vec) {
        return vec + LastWorldgenTLCorner;
    }

    public Vector2 ScreenToWorldSpace(Vector2 vec) {
        return vec / TILE_SIZE + TopLeftCorner;
    }
    public Vector2 WorldToScreenSpace(Vector2 vec) {
        return (vec - TopLeftCorner) * TILE_SIZE;
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
