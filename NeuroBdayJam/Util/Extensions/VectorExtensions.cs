using NeuroBdayJam.Util.Math;
using System.Numerics;

namespace NeuroBdayJam.Util.Extensions;
public static class VectorExtensions {

    public static Vector2 ToVector2(this (float x, float y) v) => new Vector2(v.x, v.y);
    public static (float x, float y) ToTuple(this Vector2 v) => (v.X, v.Y);

    public static Vector2 Clone(this Vector2 v) => new Vector2(v.X, v.Y);

    public static Vector2 Rotate(this Vector2 p, float rotation) {
        float s = MathF.Sin(rotation);
        float c = MathF.Cos(rotation);

        return new Vector2(p.X * c - p.Y * s, p.X * s + p.Y * c);
    }

    public static Vector2 RotateAround(this Vector2 p, Vector2 pivot, float rotation) {
        float x = p.X - pivot.X;
        float y = p.Y - pivot.Y;

        float s = MathF.Sin(rotation);
        float c = MathF.Cos(rotation);

        return new Vector2(x * c - y * s + pivot.X, x * s + y * c + pivot.Y);
    }

    public static void GetNormals(this Vector2 v, out Vector2 n1, out Vector2 n2) {
        n1 = new Vector2(-v.Y, v.X);
        n2 = new Vector2(v.Y, -v.X);
    }

    public static Vector2 ToPolar(this Vector2 v) {
        float r = v.Length();
        float phi = MathF.Atan2(v.Y, v.X).NormalizeAngle();

        return new Vector2(r, phi);
    }

    public static Vector2 FromPolar(this Vector2 polar) => new Vector2(polar.X * MathF.Cos(polar.Y), polar.X * MathF.Sin(polar.Y));

    public static Vector2 FromPolar(float radius, float angle) => new Vector2(radius * MathF.Cos(angle), radius * MathF.Sin(angle));

    internal static Vector2 ToVector2WithOffsetAndScaling(this Vector2 v, Vector2 offset, Vector2 scaling) {
        return new Vector2(offset.X + v.X * scaling.X, offset.Y + v.Y * scaling.Y);
    }
}