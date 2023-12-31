using NeuroBdayJam.Game.Entities;
using NeuroBdayJam.Game.Entities.Enemies;
using NeuroBdayJam.Game.Entities.Memories;
using NeuroBdayJam.Game.Memories;
using NeuroBdayJam.Game.Scenes;
using NeuroBdayJam.Game.World.Generation;
using NeuroBdayJam.Graphics;
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
    public Vector2 PlayerSpawn { get; set; }

    private HashSet<Entity> Entities { get; }
    public Memory? ActiveMemory { get; set; }

    public MemoryTracker MemoryTracker { get; }

    public Tileset Tileset { get; private set; }
    private ShaderResource ScanlinesShader { get; set; }
    private ShaderResource BloomShader { get; set; }

    private Vector2 TopLeftCorner { get; set; }
    private Vector2 CenterOfScreen => TopLeftCorner + new Vector2(Width, Height) / 2.0f;
    private Vector2 LastWorldgenTLCorner { get; set; }
    private Vector2 WorldgenDelta => LastWorldgenTLCorner - TopLeftCorner;
    private List<(Type entity, float spawnRate, SpawnCondition condition)> Spawnables { get; }
    public float TimeScale { get; set; }

    public delegate bool SpawnCondition(WorldTile tile);

    public GameWorld() {
        Vector2 visibleTileSize = GetVisibleTileSize();
        int tileWidth = (int)Math.Ceiling(visibleTileSize.X + 6);
        int tileHeight = (int)Math.Ceiling(visibleTileSize.Y + 6);
        WorldGenerator = new DefaultWorldGenerator(tileWidth, tileHeight);
        MemoryTracker = new MemoryTracker();

        Tiles = new WorldTile[tileWidth, tileHeight];
        Entities = new HashSet<Entity>();
        TopLeftCorner = new Vector2(0, 0);
        LastWorldgenTLCorner = TopLeftCorner;

        PlayerSpawn = new Vector2(tileWidth / 2 + 0.5f, tileHeight / 2 + 0.5f);

        Spawnables = new(){
            new(typeof(Memory), 0.0015f, tile => tile.Position.Length() > 25 && MemoryTracker.NumUncollectedMemories > 0 && ActiveMemory == null),
            new(typeof(Worm), 0.005f, _ => true),
        };

        TimeScale = 1;
    }

    public void Load() {
        if (WorldGenerator != null)
            SetFromIds(Tiles, WorldGenerator.ExportToUlongs(), 0, 0);

        ResourceManager.SoundLoader.Load("step");
        ResourceManager.SoundLoader.Load("get_memory");
        ResourceManager.SoundLoader.Load("ability_1");
        ResourceManager.SoundLoader.Load("ability_2");
        ResourceManager.SoundLoader.Load("Clips/intro_speech");
        TilesetResource tilesetResource = ResourceManager.TilesetLoader.Get("dark");
        ResourceManager.TextureAtlasLoader.Load("player_animations");
        ScanlinesShader = ResourceManager.ShaderLoader.Get("scanlines");
        BloomShader = ResourceManager.ShaderLoader.Get("bloom");

        tilesetResource.WaitForLoad();
        ScanlinesShader.WaitForLoad();
        BloomShader.WaitForLoad();

        Renderer.PostProcessShaders.Add(BloomShader);
        Renderer.PostProcessShaders.Add(ScanlinesShader);
        Tileset = tilesetResource.Resource;

        Player = new Neuro(PlayerSpawn);
        AddEntity(Player);

        //ActiveMemory = new Memory(new Vector2(5.5f, 5.5f));
        //AddEntity(ActiveMemory);

        AddEntity(new VedalTerminal(new Vector2(Width / 2 + 0.5f, Height / 2 + 0.5f - 2)));

        // TODO TESTING
        //AddEntity(new Worm(new Vector2(5.5f, 5.5f)));

        ((GameScene)GameManager.Scene).Cutscene = new Cutscene("dialogue_intro", "Clips/intro_speech");
    }

    public void Unload() {
        Renderer.PostProcessShaders.Remove(BloomShader);
        Renderer.PostProcessShaders.Remove(ScanlinesShader);

        foreach (Entity entity in Entities.ToList()) {
            if (entity.World != null)
                entity.Unload();
        }
    }

    internal void Update(float dT) {
        dT *= TimeScale;

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
                if (entity is not Neuro and not VedalTerminal) {
                    entity.Unload();
                    Entities.Remove(entity);
                }
            } else
                entity.Update(dT);
        }
        MemoryTracker.Update(this, dT);

        //Player.Update(dT);
    }

    internal void Render(float dT) {
        RlGl.rlPushMatrix();
        Vector2 visibleTileSize = GetVisibleTileSize();
        int xOffset = ((int)(Width - visibleTileSize.X) / 2);
        int yOffset = ((int)(Height - visibleTileSize.Y) / 2);

        RlGl.rlTranslatef(-(TopLeftCorner.X + xOffset) * TILE_SIZE, -(TopLeftCorner.Y + yOffset) * TILE_SIZE, 0);

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

    internal void RenderPostProcessed(ShaderResource shader, float dT) {
        RlGl.rlPushMatrix();
        Vector2 visibleTileSize = GetVisibleTileSize();
        int xOffset = ((int)(Width - visibleTileSize.X) / 2);
        int yOffset = ((int)(Height - visibleTileSize.Y) / 2);
        RlGl.rlTranslatef(-(TopLeftCorner.X + xOffset) * TILE_SIZE, -(TopLeftCorner.Y + yOffset) * TILE_SIZE, 0);

        if (shader.Key == "scanlines")
            RenderScanlinesObjects(dT);
        else if (shader.Key == "bloom")
            RenderBloomObjects(dT);

        RlGl.rlPopMatrix();
    }

    private void RenderBloomObjects(float dT) {
        foreach (Entity entity in Entities.Where(e => e is Enemy || e is Memory).OrderByDescending(e => e.ZIndex).ToList()) {
            if (!entity.IsDead && entity.World != null)
                entity.Render(dT);
        }
    }

    private void RenderScanlinesObjects(float dT) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tiles[x, y].Render(dT);
            }
        }

        foreach (Entity entity in Entities.Where(e => !(e is Enemy || e is Memory)).OrderByDescending(e => e.ZIndex).ToList()) {
            if (!entity.IsDead && entity.World != null)
                entity.Render(dT);
        }
    }

    public void AddEntity(Entity entity) {
        Entities.Add(entity);
    }

    public WorldTile? GetTile(Vector2 position) {
        position = WorldToTileIndexSpace(position);

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

                    if (entityType == typeof(Memory))
                        ActiveMemory = (Memory)newEntity;

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
        Vector2 visibleTileSize = GetVisibleTileSize();
        Vector2 offset = (new Vector2(Width, Height) - visibleTileSize) / 2;
        return vec / TILE_SIZE + TopLeftCorner - offset;
    }
    public Vector2 WorldToScreenSpace(Vector2 vec) {
        Vector2 visibleTileSize = GetVisibleTileSize();
        Vector2 offset = (new Vector2(Width, Height) - visibleTileSize) / 2;
        return (vec - TopLeftCorner + offset) * TILE_SIZE;
    }

    public static Vector2 GetVisibleTileSize() {
        float visibleTileWidth = Application.BASE_WIDTH / TILE_SIZE;
        float visibleTileHeight = Application.BASE_HEIGHT / TILE_SIZE;

        return new(visibleTileWidth, visibleTileHeight);
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
