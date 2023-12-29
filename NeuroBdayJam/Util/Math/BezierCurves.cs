using System.Numerics;

namespace NeuroBdayJam.Util.Math;
public static class BezierCurves {

    public static Vector2 Quadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
        float u = 1 - t;

        Vector2 p = u * u * p0 +
            2 * u * t * p1 +
            t * t * p2;

        return p;
    }

    public static Vector2 Cubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
        float u = 1 - t;

        Vector2 p = u * u * u * p0 +
            3 * u * u * t * p1 +
            3 * u * t * t * p2 +
            t * t * t * p3;

        return p;
    }
}
