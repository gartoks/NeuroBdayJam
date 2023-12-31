using System.Numerics;

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

    protected override ulong ReplaceTile(ulong tile, int x, int y) {
        if (x >= Width / 2 - 1 && x <= Width / 2 + 1 && y >= Height / 2 - 3 && y <= Height / 2 - 1)
            return 2;

        Vector2 center = new Vector2(Width / 2f, Height / 2f);
        float distance = Vector2.Distance(center, new Vector2(x, y));

        if (distance < 10)
            return 1;

        return tile;
    }
}
