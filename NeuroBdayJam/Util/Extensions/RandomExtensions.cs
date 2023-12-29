using NeuroBdayJam.Util.Math.Random;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Util.Extensions;
public static class RandomExtensions {
    public static Vector2 NextVector(this IRandomGenerator rand, float min = 0, float max = 1) {
        return new Vector2(rand.NextFloat(min, max), rand.NextFloat(min, max));
    }

    public static float NextGaussian(this IRandomGenerator rand, float mean = 0, float stdDev = 1) {
        float u1 = rand.NextFloat();
        float u2 = rand.NextFloat();
        double randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Sin(2.0f * MathF.PI * u2);
        return mean + stdDev * (float)randStdNormal;
    }

    public static float NextAngle(this IRandomGenerator rand) {
        return rand.NextFloat() * MathF.Tau;
    }

    public static Vector2 NextRandomInCircleUniformly(this IRandomGenerator rand, float maxRadius, float minRadius = -1) {
        if (minRadius < 0)
            minRadius = 0;

        float angle = rand.NextAngle();
        float r = MathF.Sqrt(rand.NextFloat()) * (maxRadius - minRadius) + minRadius;
        float x = r * MathF.Cos(angle);
        float y = r * MathF.Sin(angle);

        return new Vector2(x, y);
    }

    public static Vector2 NextRandomInCircleCentered(this IRandomGenerator rand, float maxRadius, float minRadius = -1) {
        if (minRadius < 0)
            minRadius = 0;

        float angle = rand.NextAngle();
        float r = rand.NextFloat() * (maxRadius - minRadius) + minRadius;
        float x = r * MathF.Cos(angle);
        float y = r * MathF.Sin(angle);

        return new Vector2(x, y);
    }

    public static Color NextColor(this IRandomGenerator random) => new Color(random.Next(256), random.Next(256), random.Next(256), 255);
}