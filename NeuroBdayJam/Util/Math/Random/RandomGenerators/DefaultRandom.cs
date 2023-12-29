namespace NeuroBdayJam.Util.Math.Random;
public class DefaultRandom : IRandomGenerator {
    public int Seed { get; private set; }

    private System.Random Random { get; set; }

    public DefaultRandom()
        : this((int)new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds) { }

    public DefaultRandom(int seed) {
        SetSeed(seed);
    }

    public void SetSeed(int seed) {
        Seed = seed;
        Random = new System.Random(seed);
    }

    public IRandomGenerator CreateNew(int seed) => new DefaultRandom(seed);

    public uint NextUint() {
        return (uint)Next(1 << 30) << 2 | (uint)Next(1 << 2);
    }

    public int Next() {
        return Random.Next();
    }

    public int Next(int minValue, int maxValue) {
        return Random.Next(minValue, maxValue);
    }

    public int Next(int maxValue) {
        return Random.Next(maxValue);
    }

    public double NextDouble() {
        return Random.NextDouble();
    }

    public double NextDouble(double max) {
        return NextDouble() * max;
    }

    public double NextDouble(double min, double max) {
        return min + NextDouble(max - min);
    }

    public float NextFloat() {
        return Random.NextSingle();
    }

    public float NextFloat(float max) {
        return NextFloat() * max;
    }

    public float NextFloat(float min, float max) {
        return min + NextFloat(max - min);
    }

    public void NextBytes(byte[] buffer) {
        Random.NextBytes(buffer);
    }

    public bool NextBool() {
        return Next() % 2 == 0;
    }

    public bool NextChance(double chanceForTrue) {
        return NextDouble() < chanceForTrue;
    }
}