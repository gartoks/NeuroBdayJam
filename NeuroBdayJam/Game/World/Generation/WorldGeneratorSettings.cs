namespace NeuroBdayJam.Game.World.Generation;
internal sealed class WorldGeneratorSettings {

    public bool GenerateEverything { get; set; }
    public string Ruleset { get; set; }

    public WorldGeneratorSettings(bool generateEverything, string ruleset) {
        GenerateEverything = generateEverything;
        Ruleset = ruleset;
    }
}
