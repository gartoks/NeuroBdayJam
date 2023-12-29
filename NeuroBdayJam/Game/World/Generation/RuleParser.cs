using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using System.Numerics;

namespace NeuroBdayJam.Game.World.Generation;

internal class RuleParser {
    List<Tile> Tiles;

    public RuleParser(){
        Tiles = new();
    }

    enum SymmetryType{
        Full,
        Horizontal,
        Vertical,
        None
    }

    enum TransformationType{
        All,
        None,
        MirrorHorizontal,
        MirrorVertical,
    }


    struct Tile{
        public int Id;
        public int[] EdgeTypes;

        public Tile(){
            EdgeTypes = new int[4];
        }
    }

    public void ParseTile(string line){
        string[] lineParts = line.Split(" ");
        Tile tile = new Tile();

        tile.Id = int.Parse(lineParts[0]);
        for (int i=2; i<2+4; i++){
            tile.EdgeTypes[i-2] = int.Parse(lineParts[i]);
        }

        Tiles.Add(tile);
    }
    public void RotateTile(int id){
        Tile tile = Tiles.Where((Tile t) => t.Id == id).First();
        Tiles.Add(new(){
            Id = 0,
            EdgeTypes = new int[4]{
                tile.EdgeTypes[(int)WorldGenerator.Side.Left],
                tile.EdgeTypes[(int)WorldGenerator.Side.Top],
                tile.EdgeTypes[(int)WorldGenerator.Side.Right],
                tile.EdgeTypes[(int)WorldGenerator.Side.Bottom],
            }
        });
        Tiles.Add(new(){
            Id = 0,
            EdgeTypes = new int[4]{
                tile.EdgeTypes[(int)WorldGenerator.Side.Bottom],
                tile.EdgeTypes[(int)WorldGenerator.Side.Left],
                tile.EdgeTypes[(int)WorldGenerator.Side.Top],
                tile.EdgeTypes[(int)WorldGenerator.Side.Right],
            }
        });
        Tiles.Add(new(){
            Id = 0,
            EdgeTypes = new int[4]{
                tile.EdgeTypes[(int)WorldGenerator.Side.Right],
                tile.EdgeTypes[(int)WorldGenerator.Side.Bottom],
                tile.EdgeTypes[(int)WorldGenerator.Side.Left],
                tile.EdgeTypes[(int)WorldGenerator.Side.Top],
            }
        });
    }

    public void Parse(string ruleset){
        foreach (string line in ruleset.Split("\n")){
            if (line.Length == 0) continue;
            if (line[0] == '#') continue;
            string[] lineParts = line.Split(" ");

            if(lineParts[0] == "R"){
                RotateTile(int.Parse(lineParts[1]));
            }
            else{
                ParseTile(line);
            }
        }

        Console.WriteLine(Tiles);
    }

    public Dictionary<(int, WorldGenerator.Side), ulong> Export(){
        Dictionary<(int, WorldGenerator.Side), ulong> IdAndSideToPossibleNeighbours = new();
        
        var AddOrAdjustRule = (int id, WorldGenerator.Side side, ulong possibleValuesBitmap) => {
            if (!IdAndSideToPossibleNeighbours.ContainsKey(new(id, side))){
                IdAndSideToPossibleNeighbours.Add(new(id, side), possibleValuesBitmap);
            }
            else{
                IdAndSideToPossibleNeighbours[new(id, side)] |= possibleValuesBitmap;
            }
        };

        AddOrAdjustRule(0, WorldGenerator.Side.Top, ulong.MaxValue);
        AddOrAdjustRule(0, WorldGenerator.Side.Left, ulong.MaxValue);
        AddOrAdjustRule(0, WorldGenerator.Side.Bottom, ulong.MaxValue);
        AddOrAdjustRule(0, WorldGenerator.Side.Right, ulong.MaxValue);

        // make all ids unique
        for (int i=0; i<Tiles.Count; i++){
            Tile t = Tiles[i];
            t.Id = i+1;
            Tiles[i] = t;

            AddOrAdjustRule(t.Id, WorldGenerator.Side.Top, 0);
            AddOrAdjustRule(t.Id, WorldGenerator.Side.Left, 0);
            AddOrAdjustRule(t.Id, WorldGenerator.Side.Bottom, 0);
            AddOrAdjustRule(t.Id, WorldGenerator.Side.Right, 0);
        }

        foreach (Tile t1 in Tiles){
            foreach (Tile t2 in Tiles){
                if (t1.EdgeTypes[(int)WorldGenerator.Side.Top] == t2.EdgeTypes[(int)WorldGenerator.Side.Bottom]){
                    AddOrAdjustRule(t1.Id, WorldGenerator.Side.Top, (ulong)1 << (t2.Id-1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.Side.Bottom, (ulong)1 << (t1.Id-1));
                }
                if (t1.EdgeTypes[(int)WorldGenerator.Side.Bottom] == t2.EdgeTypes[(int)WorldGenerator.Side.Top]){
                    AddOrAdjustRule(t1.Id, WorldGenerator.Side.Bottom, (ulong)1 << (t2.Id-1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.Side.Top, (ulong)1 << (t1.Id-1));
                }
                if (t1.EdgeTypes[(int)WorldGenerator.Side.Left] == t2.EdgeTypes[(int)WorldGenerator.Side.Right]){
                    AddOrAdjustRule(t1.Id, WorldGenerator.Side.Left, (ulong)1 << (t2.Id-1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.Side.Right, (ulong)1 << (t1.Id-1));
                }
                if (t1.EdgeTypes[(int)WorldGenerator.Side.Right] == t2.EdgeTypes[(int)WorldGenerator.Side.Left]){
                    AddOrAdjustRule(t1.Id, WorldGenerator.Side.Right, (ulong)1 << (t2.Id-1));
                    AddOrAdjustRule(t2.Id, WorldGenerator.Side.Left, (ulong)1 << (t1.Id-1));
                }
            }
        }

        return IdAndSideToPossibleNeighbours;
    }

    // Dictionary<(int, Side), ulong> IdAndSideToPossibleNeighbours = new(){
    //     {new(0, Side.Top), ulong.MaxValue},
    //     {new(0, Side.Left), ulong.MaxValue},
    //     {new(0, Side.Bottom), ulong.MaxValue},
    //     {new(0, Side.Right), ulong.MaxValue},


    //     {new(1, Side.Top), 0b101},
    //     {new(1, Side.Left), 0b001},
    //     {new(1, Side.Bottom), 0b101},
    //     {new(1, Side.Right), 0b011},

    //     {new(2, Side.Top), 0b100},
    //     {new(2, Side.Left), 0b101},
    //     {new(2, Side.Bottom), 0b101},
    //     {new(2, Side.Right), 0b100},

    //     {new(3, Side.Top), 0b111},
    //     {new(3, Side.Left), 0b111},
    //     {new(3, Side.Bottom), 0b111},
    //     {new(3, Side.Right), 0b111},
    // };
}