namespace NeuroBdayJam.Util.Math.Random;
public class LehmerRandom : IRandomGenerator {
    public int Seed { get; private set; }

    private uint State { get; set; }

    public LehmerRandom(int seed) {
        Seed = seed;
        State = BitConverter.ToUInt32(BitConverter.GetBytes(seed), 0);
    }

    public void SetSeed(int seed) {
        Seed = seed;
        State = BitConverter.ToUInt32(BitConverter.GetBytes(seed), 0);
    }

    public IRandomGenerator CreateNew(int seed) => new LehmerRandom(seed);

    public uint NextUint() {
        State += 0x120fc15;
        ulong tmp = (ulong)State * 0x4a39b70d;
        uint m1 = (uint)(tmp >> 32 ^ tmp);
        tmp = (ulong)m1 * 0x12fad5c9;
        return (uint)(tmp >> 32 ^ tmp);
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