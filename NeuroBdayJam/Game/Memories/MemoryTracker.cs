using NeuroBdayJam.Util.Extensions;

namespace NeuroBdayJam.Game.Memories;
internal static class MemoryTracker {
    public struct MemoryData {
        public string Name;
    }

    public static int NumMemories => AllMemories.Count;
    public static int NumMemoriesCollected => CollectedMemories.Count;
    public static int NumTemproaryMemories => TemporaryMemories.Count;
    public static int NumUncollectedMemories => UncollectedMemories.Count;

    public static List<MemoryData> AllMemories { get; }
    private static List<int> CollectedMemories;
    private static List<int> TemporaryMemories;
    private static List<int> UncollectedMemories;

    static MemoryTracker() {
        AllMemories = new(){
            new() {
                Name = "Memory 1",
            },
            new() {
                Name = "Memory 2",
            },
            new() {
                Name = "Memory 3",
            }
        };
        CollectedMemories = new();
        UncollectedMemories = AllMemories.Select((m, i) => i).ToList();
        TemporaryMemories = new();
    }

    public static bool IsMemoryCollected(string name) {
        return true;    // TODO for testing
        return CollectedMemories.Contains(AllMemories.FindIndex(m => m.Name == name));
    }

    public static int GetNextUncollectedMemory() {
        return UncollectedMemories.First();
    }

    public static void CollectMemory(int index) {
        UncollectedMemories.Remove(index);
        TemporaryMemories.Add(index);
    }

    public static void InternalizeMemories() {
        CollectedMemories.InsertRange(CollectedMemories.Count, TemporaryMemories);
        TemporaryMemories.Clear();
    }

    public static void LooseTemporaryMemories() {
        UncollectedMemories.InsertRange(UncollectedMemories.Count, TemporaryMemories);
        UncollectedMemories.Sort();
        TemporaryMemories.Clear();
    }
}