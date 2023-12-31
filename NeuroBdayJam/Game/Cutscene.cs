using NeuroBdayJam.App;
using NeuroBdayJam.Audio;
using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.Game.Utils;
using NeuroBdayJam.ResourceHandling;
using Raylib_CsLo;
using System.Globalization;
using System.Numerics;

namespace NeuroBdayJam.Game;
internal class Cutscene {
    private string Key { get; }
    private string? SoundWhenFinished { get; set; }

    private IReadOnlyDictionary<int, string> Dialogue { get; set; }

    public bool IsFinished => CurrentDialogueIndex >= Dialogue.Count;

    private int CurrentDialogueIndex { get; set; }
    private float ContinueCooldown { get; set; }

    private float TimeSinceAdvanceDialogue { get; set; }
    private int ShownCharacters => (int)(TimeSinceAdvanceDialogue * 20);

    private GUIImage TutelTalkingImage { get; set; }
    private GuiPanel DialoguePanel { get; set; }
    private GuiDynamicLabel DialogueLabel { get; set; }

    public Cutscene(string resourceKey, string soundWhenFinished) {
        Key = resourceKey;
        SoundWhenFinished = soundWhenFinished;

        Dialogue = ResourceManager.TextLoader.Get(resourceKey).WaitForLoad().ToDictionary(kvp => int.Parse(kvp.Key, CultureInfo.InvariantCulture), kvp => kvp.Value);

        CurrentDialogueIndex = 0;
        TimeSinceAdvanceDialogue = 0;
        ContinueCooldown = 0;

        TutelTalkingImage = new GUIImage(0.05f * Application.BASE_WIDTH, 0.425f * Application.BASE_HEIGHT, 8, GameManager.MiscAtlas.GetSubTexture("tutel_talk")!);

        DialoguePanel = new GuiPanel("0.025 0.55 0.95 0.4", "panel", Vector2.Zero);
        DialogueLabel = new GuiDynamicLabel(0.2f * Application.BASE_WIDTH, 0.625f * Application.BASE_HEIGHT, string.Empty, 100, Vector2.Zero);

        Input.RegisterHotkey(GameHotkeys.ADVANCE_DIALOGUE, KeyboardKey.KEY_SPACE);
    }

    internal void Update(float dT) {
        if (IsFinished)
            return;

        ContinueCooldown -= dT;
        TimeSinceAdvanceDialogue += dT;

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Input.IsMouseButtonActive(MouseButton.MOUSE_BUTTON_LEFT)) {
            if (ContinueCooldown <= 0 && ShownCharacters >= Dialogue[CurrentDialogueIndex].Length) {
                ContinueCooldown = 0.25f;
                Input.WasMouseHandled[MouseButton.MOUSE_BUTTON_LEFT] = true;

                CurrentDialogueIndex++;
                TimeSinceAdvanceDialogue = 0;

                if (IsFinished) {
                    Input.UnregisterHotkey(GameHotkeys.ADVANCE_DIALOGUE);

                    if (SoundWhenFinished != null)
                        AudioManager.PlaySound(SoundWhenFinished);
                }
            } else if (ShownCharacters <= Dialogue[CurrentDialogueIndex].Length) {
                TimeSinceAdvanceDialogue = 1000;
            }
        }
    }

    internal void Render(float dT) {
        if (IsFinished)
            return;

        string fullDialogue = Dialogue[CurrentDialogueIndex];
        string shownDialogue = fullDialogue[..Math.Min(ShownCharacters, fullDialogue.Length)];

        DialoguePanel.Draw();

        DialogueLabel.Text = shownDialogue;
        DialogueLabel.Draw();


        TutelTalkingImage.Draw();

        //SubTexture? tutelTalkTexture = GameManager.MiscAtlas.GetSubTexture("tutel_talk");
        //tutelTalkTexture?.Draw(new Rectangle(0.05f * Application.BASE_WIDTH, 0.95f * Application.BASE_WIDTH, 0.2f * Application.BASE_WIDTH, 0.2f * Application.BASE_WIDTH), new Vector2(0, 1));
    }
}
