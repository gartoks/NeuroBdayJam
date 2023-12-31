using NeuroBdayJam.App;
using NeuroBdayJam.Audio;
using NeuroBdayJam.Game.World;

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

    private int MusicVolumeBeforeChange = 0;

    private int CurrentlyPlayingMemoryIndex = -1;
    private bool WaitingForMemoryPlay = false;

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

    public MemoryTracker() {
        CollectedMemories = new();
        UncollectedMemories = AllMemories.Select((m, i) => i).ToList();
        TemporaryMemories = new();
    }

    public bool IsMemoryCollected(string name) {
        return true;    // TODO for testing
        return CollectedMemories.Contains(AllMemories.FindIndex(m => m.Name == name)) || TemporaryMemories.Contains(AllMemories.FindIndex(m => m.Name == name));
    }

    public int GetNextUncollectedMemory() {
        return UncollectedMemories.First();
    }

    public void CollectMemory(int index) {
        UncollectedMemories.Remove(index);
        TemporaryMemories.Add(index);

        AudioManager.PlaySound(AllMemories[index].ClipFilename);
        CurrentlyPlayingMemoryIndex = index;
        MusicVolumeBeforeChange = Application.Settings.MusicVolume;
        Application.Settings.MusicVolume = (int)((float)Application.Settings.MusicVolume * 0.3);
        WaitingForMemoryPlay = true;
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

    public void Update(GameWorld world, float dT) {
        if (CurrentlyPlayingMemoryIndex >= 0) {
            if (AudioManager.IsSoundPlaying(AllMemories[CurrentlyPlayingMemoryIndex].ClipFilename)) {
                world.TimeScale = 0;
                WaitingForMemoryPlay = false;
            } else if (!WaitingForMemoryPlay){
                world.TimeScale = 1;
                Application.Settings.MusicVolume = MusicVolumeBeforeChange;
                CurrentlyPlayingMemoryIndex = -1;
            }
        }
    }
}