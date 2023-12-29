using NeuroBdayJam.Util.Extensions;

namespace NeuroBdayJam.Util.Math.Random;

public class PerlinNoise : INoiseGenerator {
    private static int[] PERMUTATIONS { get; } = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};

    public int Seed { get; }

    public int RepeatSize { get; }
    public int Octaves { get; }
    public double Persistence { get; }

    private int[] Permutations { get; }

    public PerlinNoise(IRandomGenerator rng, int repeatSize, int octaves, double persistence) {
        Seed = rng.Seed;

        RepeatSize = repeatSize;
        Octaves = octaves;
        Persistence = persistence;

        int[] shuffled_permutations = PERMUTATIONS.Shuffle(rng).ToArray();

        Permutations = new int[512];
        for (int i = 0; i < 512; i++)
            Permutations[i] = shuffled_permutations[i & 255];
    }

    public double Generate(double x) => Generate(x, 0);

    public double Generate(double x, double y) => Generate(x, y, 0);

    public double Generate(double x, double y, double z) {
        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;
        for (int i = 0; i < Octaves; i++) {
            total += GenerateRaw(x * frequency, y * frequency, z * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= Persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }

    //public double Generate(double x, double y, double z, double w) => throw new System.NotImplementedException();

    private double GenerateRaw(double x, double y, double z) {
        if (RepeatSize > 0) {
            x = x % RepeatSize;
            y = y % RepeatSize;
            z = z % RepeatSize;
        }

        int xi = (int)x & 255;
        int yi = (int)y & 255;
        int zi = (int)z & 255;
        double xf = x - (int)x;
        double yf = y - (int)y;
        double zf = z - (int)z;

        double u = Fade(xf);
        double v = Fade(yf);
        double w = Fade(zf);

        int aaa, aba, aab, abb, baa, bba, bab, bbb;
        aaa = Permutations[Permutations[Permutations[xi] + yi] + zi];
        aba = Permutations[Permutations[Permutations[xi] + Inc(yi)] + zi];
        aab = Permutations[Permutations[Permutations[xi] + yi] + Inc(zi)];
        abb = Permutations[Permutations[Permutations[xi] + Inc(yi)] + Inc(zi)];
        baa = Permutations[Permutations[Permutations[Inc(xi)] + yi] + zi];
        bba = Permutations[Permutations[Permutations[Inc(xi)] + Inc(yi)] + zi];
        bab = Permutations[Permutations[Permutations[Inc(xi)] + yi] + Inc(zi)];
        bbb = Permutations[Permutations[Permutations[Inc(xi)] + Inc(yi)] + Inc(zi)];

        double x1, x2, y1, y2;
        x1 = Lerp(Gradient(aaa, xf, yf, zf), Gradient(baa, xf - 1, yf, zf), u);
        x2 = Lerp(Gradient(aba, xf, yf - 1, zf), Gradient(bba, xf - 1, yf - 1, zf), u);
        y1 = Lerp(x1, x2, v);

        x1 = Lerp(Gradient(aab, xf, yf, zf - 1), Gradient(bab, xf - 1, yf, zf - 1), u);
        x2 = Lerp(Gradient(abb, xf, yf - 1, zf - 1), Gradient(bbb, xf - 1, yf - 1, zf - 1), u);
        y2 = Lerp(x1, x2, v);

        return (Lerp(y1, y2, w) + 1) / 2;
    }

    private double Gradient(int hash, double x, double y, double z) {
        //return this.gradientTable[hash, 0] * x + this.gradientTable[hash, 1] * y + this.gradientTable[hash, 2] * z;

        /*int h = hash & 15;                                  // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
        double u = h < 8 ? x : y;              // If the most significant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

        double v;                                           // In Ken Perlin's original implementation this was another conditional operator (?:).  I
        // expanded it for readability.

        if (h < 4)                             // If the first and second significant bits are 0 set v = y
            v = y;
        else if (h == 12 || h == 14)// If the first and second significant bits are 1 set v = x
            v = x;
        else                                                // If the first and second significant bits are not equal (0/1, 1/0) set v = z
            v = z;

        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
        */

        // Fast
        return (hash & 0xF) switch {
            0x0 => x + y,
            0x1 => -x + y,
            0x2 => x - y,
            0x3 => -x - y,
            0x4 => x + z,
            0x5 => -x + z,
            0x6 => x - z,
            0x7 => -x - z,
            0x8 => y + z,
            0x9 => -y + z,
            0xA => y - z,
            0xB => -y - z,
            0xC => y + x,
            0xD => -y + z,
            0xE => y - x,
            0xF => -y - z,
            _ => 0,// never happens
        };
    }

    public static double Lerp(double a, double b, double t) {
        return a + t * (b - a);
    }

    private int Inc(int num) {
        num++;
        if (RepeatSize > 0)
            num %= RepeatSize;

        return num;
    }

    private double Fade(double t) {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}
