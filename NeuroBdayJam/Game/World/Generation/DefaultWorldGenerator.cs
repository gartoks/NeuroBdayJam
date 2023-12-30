namespace NeuroBdayJam.Game.World.Generation;
internal sealed class DefaultWorldGenerator : WorldGenerator {
    public DefaultWorldGenerator(int width, int height)
        : base(width, height, GetSettings()) {
    }

    private static WorldGeneratorSettings GetSettings() {
        return new WorldGeneratorSettings(true, @"
2 -> WFW WWW WFW WWW
R 2
4 -> WWW WWW WWW WWW
R 4
R 4
R 4
R 4
5 -> WFW WFW WWW WWW
R 5
6 -> WFW WFW WFW WWW
R 6
");
    }
}
