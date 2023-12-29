using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Util.Extensions;
public static class RandomExtensions {

    public static float NextSingle(this Random rand, float min = 0, float max = 1) {
        return rand.NextSingle() * (max - min) + min;
    }

    public static Vector2 NextVector(this Random rand, float min = 0, float max = 1) {
        return new Vector2(rand.NextSingle(min, max), rand.NextSingle(min, max));
    }

    public static float NextGaussian(this Random rand, float mean = 0, float stdDev = 1) {
        float u1 = rand.NextSingle();
        float u2 = rand.NextSingle();
        double randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Sin(2.0f * MathF.PI * u2);
        return mean + stdDev * (float)randStdNormal;
    }

    public static float NextAngle(this Random rand) {
        return rand.NextSingle() * MathF.Tau;
    }

    public static Vector2 NextPointOnUnitSphere(this Random rand) {
        float angle = rand.NextAngle();
        float x = MathF.Cos(angle);
        float y = MathF.Sin(angle);

        return new Vector2(x, y);
    }

    public static Vector2 NextRandomInCircleUniformly(this Random rand, float maxRadius, float minRadius = -1) {
        if (minRadius < 0)
            minRadius = 0;

        float angle = rand.NextAngle();
        float r = MathF.Sqrt(rand.NextSingle()) * (maxRadius - minRadius) + minRadius;
        float x = r * MathF.Cos(angle);
        float y = r * MathF.Sin(angle);

        return new Vector2(x, y);
    }

    public static Vector2 NextRandomInCircleCentered(this Random rand, float maxRadius, float minRadius = -1) {
        if (minRadius < 0)
            minRadius = 0;

        float angle = rand.NextAngle();
        float r = rand.NextSingle() * (maxRadius - minRadius) + minRadius;
        float x = r * MathF.Cos(angle);
        float y = r * MathF.Sin(angle);

        return new Vector2(x, y);
    }

    public static Color NextColor(this Random random) => new Color(random.Next(256), random.Next(256), random.Next(256), 255);
}