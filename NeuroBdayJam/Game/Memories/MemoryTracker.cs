using NeuroBdayJam.Audio;
using NeuroBdayJam.Game.Scenes;
using NeuroBdayJam.Game.World;
using NeuroBdayJam.ResourceHandling;

namespace NeuroBdayJam.Game.Memories;
internal class MemoryTracker {
    public struct MemoryData {
        public string Name;
        public string ClipFilename;
    }

    private GameWorld World { get; }

    public int NumMemories => AllMemories.Count;
    public int NumMemoriesCollected => CollectedMemories.Count;
    public bool HoldsMemory => TemporaryMemory >= 0;
    public int NumUncollectedMemories => UncollectedMemories.Count;

    public static List<MemoryData> AllMemories { get; }
    private List<int> CollectedMemories;
    private List<int> UncollectedMemories;
    private int TemporaryMemory;

    static MemoryTracker() {
        AllMemories = new(){
            new() {
                Name = "Memory 1",
                ClipFilename = "Clips/memory_1"
            },
            new() {
                Name = "Memory 2",
                ClipFilename = "Clips/memory_2"
            },
            new() {
                Name = "Memory 3",
                ClipFilename = "Clips/memory_3"
            },
            new() {
                Name = "Memory 4",
                ClipFilename = "Clips/memory_4"
            },
            new() {
                Name = "Memory 5",
                ClipFilename = ""
            }
        };
    }

    public MemoryTracker(GameWorld world) {
        World = world;
        CollectedMemories = new();
        UncollectedMemories = AllMemories.Select((m, i) => i).ToList();
        TemporaryMemory = -1;
    }

    public bool IsMemoryCollected(string name) {
        return CollectedMemories.Contains(AllMemories.FindIndex(m => m.Name == name)) || TemporaryMemory == AllMemories.FindIndex(m => m.Name == name);
    }

    public int GetNextUncollectedMemory() {
        return UncollectedMemories.First();
    }

    public bool TryCollectMemory(int index) {
        if (HoldsMemory)
            return false;

        UncollectedMemories.Remove(index);
        TemporaryMemory = index;

        if (index == 3) {
            AudioManager.ClearMusic();
            GameManager.Music.Clear();
            GameManager.Music.Add(ResourceManager.MusicLoader.Get("music_2"));
        }

        return true;
    }

    public void InternalizeMemory() {
        int tempMemIdx = TemporaryMemory;
        CollectedMemories.Add(tempMemIdx);

        string memClipFile = AllMemories[tempMemIdx].ClipFilename;
        ((GameScene)GameManager.Scene).Cutscene = new Cutscene($"dialogue_memory_{tempMemIdx + 1}", () => {
            if (tempMemIdx == 3)
                World.Player.SwitchCharacter();

            if (tempMemIdx == 4)
                GameManager.SetScene(new MainMenuScene());

            if (!string.IsNullOrEmpty(memClipFile))
                AudioManager.PlaySound(memClipFile, 1f);
        });

        TemporaryMemory = -1;
    }

    public void LooseTemporaryMemories() {
        UncollectedMemories.Add(TemporaryMemory);
        UncollectedMemories.Sort();
        TemporaryMemory = -1;
    }
}