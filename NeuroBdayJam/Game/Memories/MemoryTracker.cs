using System.Resources;
using NeuroBdayJam.Audio;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;

namespace NeuroBdayJam.Game.Memories;
internal class MemoryTracker {
    public struct MemoryData {
        public string Name;
        public string ClipFilename;
    }

    public int NumMemories => AllMemories.Count;
    public int NumMemoriesCollected => CollectedMemories.Count;
    public int NumTemproaryMemories => TemporaryMemories.Count;
    public int NumUncollectedMemories => UncollectedMemories.Count;

    public static List<MemoryData> AllMemories { get; }
    private List<int> CollectedMemories;
    private List<int> TemporaryMemories;
    private List<int> UncollectedMemories;

    private int CurrentlyPlayingMemoryIndex = -1;

    static MemoryTracker() {
        AllMemories = new(){
            new() {
                Name = "Memory 1",
                ClipFilename = "Clips/Never_call_Neuro_a_Bro"
            },
            new() {
                Name = "Memory 2",
                ClipFilename = "Clips/Never_call_Neuro_a_Bro"
            },
            new() {
                Name = "Memory 3",
                ClipFilename = "Clips/Never_call_Neuro_a_Bro"
            }
        };
    }

    public MemoryTracker(){
        CollectedMemories = new();
        UncollectedMemories = AllMemories.Select((m, i) => i).ToList();
        TemporaryMemories = new();
    }

    public bool IsMemoryCollected(string name) {
        return true;    // TODO for testing
        return CollectedMemories.Contains(AllMemories.FindIndex(m => m.Name == name));
    }

    public int GetNextUncollectedMemory() {
        return UncollectedMemories.First();
    }

    public void CollectMemory(int index) {
        UncollectedMemories.Remove(index);
        TemporaryMemories.Add(index);

        AudioManager.PlaySound(AllMemories[index].ClipFilename);
        CurrentlyPlayingMemoryIndex = index;
    }

    public void InternalizeMemories() {
        CollectedMemories.InsertRange(CollectedMemories.Count, TemporaryMemories);
        TemporaryMemories.Clear();
    }

    public void LooseTemporaryMemories() {
        UncollectedMemories.InsertRange(UncollectedMemories.Count, TemporaryMemories);
        UncollectedMemories.Sort();
        TemporaryMemories.Clear();
    }

    public void Update(GameWorld world, float dT){
        if (CurrentlyPlayingMemoryIndex >= 0){
            if (AudioManager.IsSoundPlaying(AllMemories[CurrentlyPlayingMemoryIndex].ClipFilename)){
                world.TimeScale = 0;
            }
            else{
                world.TimeScale = 1;
            }
        }
    }
}