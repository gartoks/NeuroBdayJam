using NeuroBdayJam.Util.Extensions;
using NeuroBdayJam.Util.Math;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Util;
internal class Collisions {

    public static Vector2 ResolveCollisionCircleRects(Vector2 position, float radius, IEnumerable<Rectangle> colliders) {
        Rectangle aabb = new Rectangle(position.X - radius, position.Y - radius, radius * 2, radius * 2);
        (Vector2 p0, Vector2 p1)[] rectLines = MathUtils.LinesFromRect(new Vector2(1, 1));

        float minDistance = float.MaxValue;
        Rectangle? closestCollider = null;
        foreach (Rectangle collider in colliders) {
            Vector2 colliderCenter = collider.Center();
            float distanceSqr = Vector2.DistanceSquared(position, colliderCenter);
            if (Raylib.CheckCollisionRecs(collider, aabb) && distanceSqr < minDistance) {
                minDistance = distanceSqr;
                closestCollider = collider;
            }
        }

        if (closestCollider == null)
            return Vector2.Zero;

        Vector2 center = closestCollider.Value.Center();
        Vector2 relPos = position - center;

        Vector2 smallestMtv = new Vector2(float.MaxValue, float.MaxValue);
        for (int i = 0; i < rectLines.Length; i++) {
            (Vector2 p0, Vector2 p1) line = rectLines[i];
            (line.p1 - line.p0).GetNormals(out Vector2 n0, out Vector2 n1);
            Vector2 closestCirclePoint = relPos + radius * Vector2.Normalize(n1);
            Vector2 closestPoint = MathUtils.GetClosesPointOnLine(closestCirclePoint, line.p0, line.p1);

            Vector2 mtv = closestPoint - closestCirclePoint;

            if (mtv.LengthSquared() < smallestMtv.LengthSquared()) {
                smallestMtv = mtv;
            }
        }
        return smallestMtv;
    }

}
