using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Util;
internal static class RaylibUtil {

    public static void DrawRectLines(Vector2 position, Vector2 size, float rotation, int thickness, Color color) {
        Vector2[] points = new Vector2[] {
            new Vector2(position.X - size.X / 2f, position.Y - size.Y / 2f).RotateAround(position, rotation),
            new Vector2(position.X + size.X / 2f, position.Y - size.Y / 2f).RotateAround(position, rotation),
            new Vector2(position.X + size.X / 2f, position.Y + size.Y /2f).RotateAround(position, rotation),
            new Vector2(position.X - size.X / 2f, position.Y + size.Y / 2f).RotateAround(position, rotation),
        };


        for (int i = 0; i < points.Length; i++) {
            Raylib.DrawLineEx(points[i], points[(i + 1) % points.Length], thickness, color);
        }
    }

}
