using NeuroBdayJam.Util.Extensions;
using Raylib_CsLo;

namespace NeuroBdayJam.Util;
internal class ColorGradient {
    private List<(float t, Color color)> Frames { get; }

    public ColorGradient() {
        Frames = new();
    }

    public void AddKeyframe(float t, Color color) {
        if (t < 0 || t > 1)
            throw new ArgumentOutOfRangeException(nameof(t), "t must be between 0 and 1");

        Frames.Add((t, color));

        List<(float t, Color color)> newFrames = Frames.OrderBy(Frames => Frames.t).ToList();
        Frames.Clear();
        Frames.AddRange(newFrames);
    }

    public Color GetColor(float t) {
        if (Frames.Count == 0)
            throw new InvalidOperationException("Gradient has no frames");

        if (Frames.Count == 1)
            return Frames[0].color;

        t = t % 1f;

        if (t == 0)
            return Frames[0].color;

        int frameIndex = 0;
        (float t, Color color) endFrame = Frames.Last();
        for (int i = 0; i < Frames.Count; i++) {
            (float t, Color color) frame = Frames[i];

            if (t <= frame.t) {
                frameIndex = i;
                endFrame = frame;
                break;
            }
        }

        (float t, Color color) startFrame = Frames[frameIndex - 1];

        float dtFrame = endFrame.t - startFrame.t;
        float dT = (t - startFrame.t) / dtFrame;

        return MiscExtensions.Lerp(startFrame.color, endFrame.color, dT);
    }


}
