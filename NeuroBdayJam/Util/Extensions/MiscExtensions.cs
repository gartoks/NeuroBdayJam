using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Util.Extensions;
internal static class MiscExtensions {

    public static bool Contains(this Rectangle rect, Vector2 pos) {
        return pos.X >= rect.x && pos.X <= rect.x + rect.width && pos.Y >= rect.y && pos.Y <= rect.y + rect.height;
    }

    public static Vector2 Center(this Rectangle rect) {
        return new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
    }

    public static Color ChangeAlpha(this Color c, int alpha) {
        return new Color(c.r, c.g, c.b, alpha);
    }
}
