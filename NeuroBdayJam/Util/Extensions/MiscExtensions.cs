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
    public static Color Lerp(Color c0, Color c1, float t, bool lerpAlpha = false) {
        int dR = c1.r - c0.r;
        int dG = c1.g - c0.g;
        int dB = c1.b - c0.b;
        int dA = c1.a - c0.a;

        int a = lerpAlpha ? c0.a + (int)(dA * t) : c0.a;
        return new Color(c0.r + (int)(dR * t), c0.g + (int)(dG * t), c0.b + (int)(dB * t), a);
    }
}
