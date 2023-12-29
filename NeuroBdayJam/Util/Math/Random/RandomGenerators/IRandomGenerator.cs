namespace NeuroBdayJam.Util.Math.Random;
public interface IRandomGenerator {
    int Seed { get; }

    void SetSeed(int seed);
    IRandomGenerator CreateNew(int seed);

    uint NextUint();
    int Next();
    int Next(int minValue, int maxValue);
    int Next(int maxValue);
    double NextDouble();
    double NextDouble(double max);
    double NextDouble(double min, double max);
    float NextFloat();
    float NextFloat(float max);
    float NextFloat(float min, float max);
    bool NextBool();
    bool NextChance(double chanceForTrue);
}