using NeuroBdayJam.Util.Extensions;
using System.Numerics;

namespace NeuroBdayJam.Util.Math;
public static class MathUtils {

    public static double Mod(double v, double mod) {
        v %= mod;
        v += mod;
        return v % mod;
    }

    public static float Mod(float v, float mod) {
        v %= mod;
        v += mod;
        return v % mod;
    }

    public static int Mod(int v, int m) {
        int a = v % m;
        return a < 0 ? a + m : a;
    }


    public static float ToDeg(this float rad) {
        return rad * 180 / MathF.PI;
    }

    public static float ToRad(this float deg) {
        return deg * MathF.PI / 180;
    }

    public static double NormalizeAngle(this double a) {
        return Mod(a, System.Math.Tau);
    }

    public static void NormalizeAngle(ref double a) {
        a = Mod(a, System.Math.Tau);
    }

    public static float NormalizeAngle(this float a) {
        return Mod(a, MathF.Tau);
    }

    public static void NormalizeAngle(ref float a) {
        a = Mod(a, MathF.Tau);
    }


    public static float Clamp01(float value) => Clamp(value, 0f, 1f);

    public static float Clamp(float value, float min, float max) => value > max ? max : (value < min ? min : value);

    public static int Clamp(int v, int min, int max) => v > max ? max : (v < min ? min : v);

    public static int FloorToInt(this float v) => (v > 0) ? ((int)v) : (((int)v) - 1);

    public static int CeilToInt(this float v) => v > 0 ? ((int)v) + 1 : (int)v;

    public static int RoundToInt(this float v) => FloorToInt(v + 0.5f);

    public static bool InRange(this float v, float min, float max) {
        return v >= min && v <= max;
    }



    public static (float x, float y) PolarToCarthesianCoordinates(float radius, float angle) => (radius * MathF.Cos(angle), radius * MathF.Sin(angle));

    public static (float radius, float angle) CarthesianToPolarCoordinates(float x, float y) => (MathF.Sqrt(x * x + y * y), MathF.Atan2(y, x));


    public static Vector2 Remap(this Vector2 value, Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2) {
        return Lerp(to2, from2, (value - from1) / (to1 - from1));
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 t) {
        return a + (b - a) * t;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2) {
        return Lerp(to2, from2, (value - from1) / (to1 - from1));
    }

    public static float Lerp(float a, float b, float t) {
        return a + (b - a) * t;
    }


    public static float NormalDistributionProbabilityDensity(float x, float mean, float stdDeviation) {
        float a = (x - mean) * (x - mean);
        float b = 2 * stdDeviation * stdDeviation;
        return 1f / MathF.Sqrt(b * MathF.PI) * MathF.Exp(-a / b);
    }


    public static bool IsPointInPolygon(IReadOnlyList<Vector2> vertices, Vector2 point) {
        bool result = false;
        int j = vertices.Count - 1;
        for (int i = 0; i < vertices.Count; i++) {
            if (vertices[i].Y < point.Y && vertices[j].Y >= point.Y || vertices[j].Y < point.Y && vertices[i].Y >= point.Y) {
                if (vertices[i].X + (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) * (vertices[j].X - vertices[i].X) < point.X) {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    public static float AreaSign(Vector2 p1, Vector2 p2, Vector2 p3) {
        return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
    }



    public static float CalculateSlope(Vector2 p1, Vector2 p2) {
        // Calculate the differences in the X and Y coordinates
        float deltaX = p2.X - p1.X;
        float deltaY = p2.Y - p1.Y;

        // Calculate the angle in radians
        float angle = MathF.Atan2(deltaY, deltaX);

        // Ensure the angle is in the range [0, 2π)
        if (angle < 0) {
            angle += MathF.Tau;
        }

        return angle;
    }

    public static bool LinesIntersectionPointPointEdge(Vector2 s0, Vector2 e0, Vector2 s1, Vector2 e1, out Vector2 intersectionPoint) {
        float s, t;
        s = (-e0.Y * (s0.X - s1.X) + e0.X * (s0.Y - s1.Y)) / (-e1.X * e0.Y + e0.X * e1.Y);
        t = (e1.X * (s0.Y - s1.Y) - e1.Y * (s0.X - s1.X)) / (-e1.X * e0.Y + e0.X * e1.Y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1) {
            intersectionPoint = s0 + (t * s1);
            return true;
        } else {
            intersectionPoint = Vector2.Zero;
            return false;
        }
    }

    public static bool LinesIntersectionPoint2Points(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 intersectionPoint) {
        Vector2 e0 = p1 - p0;
        Vector2 e1 = p3 - p2;

        float s, t;
        s = (-e0.Y * (p0.X - p2.X) + e0.X * (p0.Y - p2.Y)) / (-e1.X * e0.Y + e0.X * e1.Y);
        t = (e1.X * (p0.Y - p2.Y) - e1.Y * (p0.X - p2.X)) / (-e1.X * e0.Y + e0.X * e1.Y);


        if (s >= 0 && s <= 1 && t >= 0 && t <= 1) {
            intersectionPoint = p0 + (t * e0);
            return true;
        } else {
            intersectionPoint = Vector2.Zero;
            return false;
        }
    }

    public static bool LineIntersectionValuesPointEdge(Vector2 s0, Vector2 e0, Vector2 s1, Vector2 e1, out float t0, out float t1) {
        Vector2 d0 = e0 - s0;
        Vector2 d1 = e1 - s1;

        if (d0.X == 0 || ((d0.Y / d0.X) * d1.X - d1.Y) == 0) {
            float eDe = d0.X / d0.Y;
            t1 = (s1.X - s0.X - eDe * (s1.Y + s0.Y)) / (eDe * d1.Y - d0.Y);
            t0 = (1f / d0.Y) * (s1.Y + t1 * d1.Y - s0.Y);
        } else {
            float eDe = d0.Y / d0.X;
            t1 = (s1.Y - s0.Y - eDe * (s1.X + s0.X)) / (eDe * d1.X - d1.Y);
            t0 = (1f / d0.X) * (s1.X + t1 * d1.X - s0.X);
        }

        return t0 >= 0 && t0 <= 1 && t1 >= 0 && t1 <= 1;
    }

    public static bool LineIntersectionValues2Points(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out float t0, out float t1) {
        Vector2 s0 = p0;
        Vector2 e0 = p1 - p0;
        Vector2 s1 = p2;
        Vector2 e1 = p3 - p2;

        Vector2 d0 = e0 - s0;
        Vector2 d1 = e1 - s1;

        if (d0.X == 0 || ((d0.Y / d0.X) * d1.X - d1.Y) == 0) {
            float eDe = d0.X / d0.Y;
            t1 = (s1.X - s0.X - eDe * (s1.Y + s0.Y)) / (eDe * d1.Y - d0.Y);
            t0 = (1f / d0.Y) * (s1.Y + t1 * d1.Y - s0.Y);
        } else {
            float eDe = d0.Y / d0.X;
            t1 = (s1.Y - s0.Y - eDe * (s1.X + s0.X)) / (eDe * d1.X - d1.Y);
            t0 = (1f / d0.X) * (s1.X + t1 * d1.X - s0.X);
        }

        return t0 >= 0 && t0 <= 1 && t1 >= 0 && t1 <= 1;
    }

    public static int CalculateSideOfPointToLine(Vector2 p, Vector2 s, Vector2 e) {
        float v = (e.X - s.X) * (p.Y - s.Y) - (p.X - s.X) * (e.Y - s.Y);
        return System.Math.Sign(v);
    }

    public static float PointLineDistance(Vector2 p, Vector2 s, Vector2 e) {
        (e - s).GetNormals(out Vector2 n0, out Vector2 n1);

        LineIntersectionValuesPointEdge(s, e, p, p + n0, out float t0, out _);

        return (s + t0 * (e - s) - p).Length();
    }

    public static (Vector2 s, Vector2 e)[] LinesFromRect(Vector2 size) {
        Vector2 p0 = new Vector2(-size.X / 2f, -size.Y / 2f);
        Vector2 p1 = new Vector2(-size.X / 2f, size.Y / 2f);
        Vector2 p2 = new Vector2(size.X / 2f, size.Y / 2f);
        Vector2 p3 = new Vector2(size.X / 2f, -size.Y / 2f);

        return new[] { (p0, p1), (p1, p2), (p2, p3), (p3, p0) };
    }
}
