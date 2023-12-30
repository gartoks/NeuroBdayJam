
using Microsoft.Toolkit.HighPerformance;
using NeuroBdayJam.Util.Extensions;

namespace NeuroBdayJam.Game.Memories;
internal static class MemoryTracker {
    public struct MemoryData{
        public string Name;
    }

    public static List<MemoryData> AllMemories { get; }
    private static List<int> CollectedMemories;
    private static List<int> TemporaryMemories;
    private static List<int> UncollectedMemories;

    static MemoryTracker(){
        AllMemories = new(){
            new(){
                Name="Memory 1",
            },
            new(){
                Name="Memory 2",
            },
            new(){
                Name="Memory 3",
            }
        };
        CollectedMemories = new();
        UncollectedMemories = AllMemories.Select((m, i) => i).ToList();
        TemporaryMemories = new();
    }

    public static int GetRandomUncollectedMemory(){
        return UncollectedMemories.Shuffle(Random.Shared).First();
    }
    public static void CollectMemory(int index){
        UncollectedMemories.Remove(index);
        TemporaryMemories.Add(index);
    }
    public static void InternalizeMemories(){
        CollectedMemories.InsertRange(CollectedMemories.Count, TemporaryMemories);
        TemporaryMemories.Clear();
    }
    public static void LooseTemporaryMemories(){
        UncollectedMemories.InsertRange(UncollectedMemories.Count, TemporaryMemories);
        TemporaryMemories.Clear();
    }

    public static int NumMemories { get => AllMemories.Count; }
    public static int NumMemoriesCollected { get => CollectedMemories.Count(); }
    public static int NumTemproaryMemories { get => TemporaryMemories.Count(); }


}