namespace NeuroBdayJam.Util.Math.Random;

public class Squirrel3Random : IRandomGenerator {
    public int Seed { get; private set; }

    private uint State { get; set; }

    public Squirrel3Random(int seed) {
        SetSeed(seed);
    }

    public void SetSeed(int seed) {
        Seed = seed;
        State = BitConverter.ToUInt32(BitConverter.GetBytes(Seed), 0);
    }

    public IRandomGenerator CreateNew(int seed) => new Squirrel3Random(seed);

    public uint NextUint() {
        const uint BIT_NOISE1 = 0xB5297A4D;
        const uint BIT_NOISE2 = 0x68E31DA4;
        const uint BIT_NOISE3 = 0x1B56C4E9;

        uint uSeed = BitConverter.ToUInt32(BitConverter.GetBytes(Seed), 0);

        uint val = BitConverter.ToUInt32(BitConverter.GetBytes(State), 0);
        val *= BIT_NOISE1;
        val += uSeed;
        val ^= val >> 8;
        val += BIT_NOISE2;
        val ^= val << 8;
        val += BIT_NOISE3;
        val ^= val >> 8;

        State = val;

        return val;
    }

    public int Next() {
        return (int)(NextUint() % int.MaxValue);
    }

    public int Next(int minValue, int maxValue) {
        return minValue + Next(maxValue - minValue);
    }

    public int Next(int maxValue) {
        return Next() % maxValue;
    }

    public double NextDouble() {
        return Next() / (double)int.MaxValue;
    }

    public double NextDouble(double max) {
        return NextDouble() * max;
    }

    public double NextDouble(double min, double max) {
        return min + NextDouble(max - min);
    }

    public float NextFloat() {
        return (float)NextDouble();
    }

    public float NextFloat(float max) {
        return NextFloat() * max;
    }

    public float NextFloat(float min, float max) {
        return min + NextFloat(max - min);
    }

    public bool NextBool() {
        return Next() % 2 == 0;
    }

    public bool NextChance(double chanceForTrue) {
        return NextDouble() < chanceForTrue;
    }

    public double NextAngle() {
        return NextDouble() * System.Math.Tau;
    }

}
