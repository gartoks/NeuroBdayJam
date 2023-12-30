﻿using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Graphics;
internal sealed class FrameAnimator {
    private float FrameDuration { get; }

    private Dictionary<string, IDrawableResource> Frames { get; }
    private Dictionary<string, List<(float weight, IReadOnlyList<string> frames)>> FrameSequences { get; }

    private string? DefaultSequence { get; set; }
    private Random Random { get; }

    public string? ActiveSequence { get; private set; }
    private int ActiveSequenceIndex { get; set; }
    private float SequencePlayTime { get; set; }
    public int FrameIndex => (int)(SequencePlayTime / FrameDuration);
    private IDrawableResource? ActiveFrame => ActiveSequence == null ? null : Frames[FrameSequences[ActiveSequence][ActiveSequenceIndex].frames[FrameIndex]];

    public bool IsReady => ActiveSequence == null;

    public FrameAnimator(float frameDuration) {
        FrameDuration = frameDuration;

        Random = new();

        Frames = new();
        FrameSequences = new();
    }

    public void AddFrameKey(string key, IDrawableResource resource) {
        Frames[key] = resource;
    }

    public void AddFrameSequence(string sequenceKey, float selectionWeight, params string[] frameKeys) {
        if (!FrameSequences.TryGetValue(sequenceKey, out List<(float weight, IReadOnlyList<string> frames)>? frames)) {
            frames = new();
            FrameSequences[sequenceKey] = frames;
        }

        frames.Add((selectionWeight, frameKeys));
    }

    public void SetDefaultSequence(string sequenceKey) {
        if (!FrameSequences.ContainsKey(sequenceKey)) {
            Log.WriteLine($"Unable to set default sequence to '{sequenceKey}' because it does not exist.", eLogType.Error);
            return;
        }

        DefaultSequence = sequenceKey;
    }

    public void StartSequence(string sequenceKey, int frameIndex = 0) {
        if (!FrameSequences.TryGetValue(sequenceKey, out List<(float weight, IReadOnlyList<string> frames)>? sequences)) {
            Log.WriteLine($"Unable to start sequence '{sequenceKey}' because it does not exist.", eLogType.Error);
            return;
        }

        ActiveSequence = sequenceKey;
        ActiveSequenceIndex = SelectSequence(sequences);
        SequencePlayTime = frameIndex * FrameDuration;
    }

    public void Render(float dT, Rectangle bounds, float rotation, Vector2 pivot, Color color) {
        if (FrameIndex >= FrameSequences[ActiveSequence!][ActiveSequenceIndex].frames.Count) {
            ActiveSequence = null;
            return;
        }

        if (ActiveSequence == null) {
            if (DefaultSequence != null)
                StartSequence(DefaultSequence);
            else
                return;
        }

        IDrawableResource frame = ActiveFrame!;
        frame.Draw(bounds, pivot, rotation, color);

        SequencePlayTime += dT;
    }

    private int SelectSequence(IReadOnlyList<(float weight, IReadOnlyList<string> frames)> sequences) {
        if (sequences.Count == 1)
            return 0;

        float totalWeight = sequences.Sum(b => b.weight);
        float value = Random.NextSingle() * totalWeight;

        for (int i = 0; i < sequences.Count; i++) {
            (float weight, IReadOnlyList<string> frames) = sequences[i];

            value -= weight;
            if (value <= 0)
                return i;
        }

        return 0;
    }
}
