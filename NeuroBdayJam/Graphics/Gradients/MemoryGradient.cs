using NeuroBdayJam.Util;
using Raylib_CsLo;

namespace NeuroBdayJam.Graphics.Gradients;
internal sealed class MemoryGradient : ColorGradient {
    public MemoryGradient() {
        AddKeyframe(0.0f, new Color(255, 209, 253, 255));
        AddKeyframe(0.22f, new Color(253, 255, 163, 255));
        AddKeyframe(0.49f, new Color(143, 202, 255, 255));
        AddKeyframe(0.78f, new Color(184, 255, 209, 255));
        AddKeyframe(1.0f, new Color(255, 209, 253, 255));
    }
}
