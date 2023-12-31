using System.Globalization;

namespace NeuroBdayJam.Game.World.Generation;

internal class RuleParser {
    List<Tile> Tiles;

    public RuleParser() {
        Tiles = new();
    }

    enum SymmetryType {
        Full,
        Horizontal,
        Vertical,
        None
    }

    enum TransformationType {
        All,
        None,
        MirrorHorizontal,
        MirrorVertical,
    }


    struct Tile {
        public int Id;
        public string[] EdgeTypes;

        public Tile() {
            EdgeTypes = new string[4]{
                "-----------------------",
                "-----------------------",
                "-----------------------",
                "-----------------------",
            };
        }
    }

    public void ParseTile(string line) {
        string[] lineParts = line.Replace("\r", "").Split(" ");
        Tile tile = new Tile();

        if (string.IsNullOrEmpty(lineParts[0]))
            return;

        tile.Id = int.Parse(lineParts[0]);
        for (int i = 2; i < 2 + 4; i++) {
            tile.EdgeTypes[i - 2] = lineParts[i];
        }

        Tiles.Add(tile);
    }
    public void RotateTile(int id) {
        Tile tile = Tiles.Where((Tile t) => t.Id == id).First();
        Tiles.Add(new() {
            Id = 0,
            EdgeTypes = new string[4]{
                tile.EdgeTypes[(int)WorldGenerator.eSide.Left],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Top],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Right],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Bottom],
            }
        });
        Tiles.Add(new() {
            Id = 0,
            EdgeTypes = new string[4]{
                tile.EdgeTypes[(int)WorldGenerator.eSide.Bottom],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Left],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Top],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Right],
            }
        });
        Tiles.Add(new() {
            Id = 0,
            EdgeTypes = new string[4]{
                tile.EdgeTypes[(int)WorldGenerator.eSide.Right],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Bottom],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Left],
                tile.EdgeTypes[(int)WorldGenerator.eSide.Top],
            }
        });
    }

    public void Parse(string ruleset) {
        foreach (string line in ruleset.Split("\n")) {
            if (line.Length == 0) continue;
            if (line[0] == '#') continue;
            string[] lineParts = line.Split(" ");

            if (lineParts[0] == "R") {
                RotateTile(int.Parse(lineParts[1], CultureInfo.InvariantCulture));
            } else {
                ParseTile(line);
            }
        }

        Console.WriteLine(Tiles);
    }

    public Dictionary<(int, WorldGenerator.eSide), ulong> Export() {
        Dictionary<(int, WorldGenerator.eSide), ulong> IdAndSideToPossibleNeighbours = new();

        Action<int, WorldGenerator.eSide, ulong> AddOrAdjustRule = (int id, WorldGenerator.eSide side, ulong possibleValuesBitmap) => {
            if (!IdAndSideToPossibleNeighbours.ContainsKey(new(id, side))) {
                IdAndSideToPossibleNeighbours.Add(new(id, side), possibleValuesBitmap);
            } else {
                IdAndSideToPossibleNeighbours[new(id, side)] |= possibleValuesBitmap;
            }
        };

        AddOrAdjustRule(0, WorldGenerator.eSide.Top, ulong.MaxValue);
        AddOrAdjustRule(0, WorldGenerator.eSide.Left, ulong.MaxValue);
        AddOrAdjustRule(0, WorldGenerator.eSide.Bottom, ulong.MaxValue);
        AddOrAdjustRule(0, WorldGenerator.eSide.Right, ulong.MaxValue);

        // make all ids unique
        for (int i = 0; i < Tiles.Count; i++) {
            Tile t = Tiles[i];
            t.Id = i + 1;
            Tiles[i] = t;

            AddOrAdjustRule(t.Id, WorldGenerator.eSide.Top, 0);
            AddOrAdjustRule(t.Id, WorldGenerator.eSide.Left, 0);
            AddOrAdjustRule(t.Id, WorldGenerator.eSide.Bottom, 0);
            AddOrAdjustRule(t.Id, WorldGenerator.eSide.Right, 0);
        }

        foreach (Tile t1 in Tiles) {
            foreach (Tile t2 in Tiles) {
                if (t1.EdgeTypes[(int)WorldGenerator.eSide.Top] == new string(t2.EdgeTypes[(int)WorldGenerator.eSide.Bottom].Reverse().ToArray())) {
                    AddOrAdjustRule(t1.Id, WorldGenerator.eSide.Top, (ulong)1 << (t2.Id - 1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.eSide.Bottom, (ulong)1 << (t1.Id - 1));
                }
                if (t1.EdgeTypes[(int)WorldGenerator.eSide.Bottom] == new string(t2.EdgeTypes[(int)WorldGenerator.eSide.Top].Reverse().ToArray())) {
                    AddOrAdjustRule(t1.Id, WorldGenerator.eSide.Bottom, (ulong)1 << (t2.Id - 1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.eSide.Top, (ulong)1 << (t1.Id - 1));
                }
                if (t1.EdgeTypes[(int)WorldGenerator.eSide.Left] == new string(t2.EdgeTypes[(int)WorldGenerator.eSide.Right].Reverse().ToArray())) {
                    AddOrAdjustRule(t1.Id, WorldGenerator.eSide.Left, (ulong)1 << (t2.Id - 1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.eSide.Right, (ulong)1 << (t1.Id - 1));
                }
                if (t1.EdgeTypes[(int)WorldGenerator.eSide.Right] == new string(t2.EdgeTypes[(int)WorldGenerator.eSide.Left].Reverse().ToArray())) {
                    AddOrAdjustRule(t1.Id, WorldGenerator.eSide.Right, (ulong)1 << (t2.Id - 1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.eSide.Left, (ulong)1 << (t1.Id - 1));
                }
            }
        }

        return IdAndSideToPossibleNeighbours;
    }
}