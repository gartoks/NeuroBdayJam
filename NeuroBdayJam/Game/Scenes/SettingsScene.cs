using System.Diagnostics;
using System.Numerics;
using NeuroBdayJam.App;
using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.ResourceHandling;

namespace NeuroBdayJam.Game.Scenes;

internal class SettingsScene : Scene {

    private GuiLabel ScreenModeLabel { get; }
    private GuiSelector ScreenModeSelector { get; }

    private GuiLabel ResolutionLabel { get; }
    private GuiSelector ResolutionSelector { get; }

    private GuiLabel MusicVolumeLabel { get; }
    private GuiSelector MusicVolumeSelector { get; }

    private GuiLabel SoundVolumeLabel { get; }
    private GuiSelector SoundVolumeSelector { get; }

    private GuiTextButton ResetTutorialButton { get; }

    private GuiTextButton ApplyButton { get; }
    private GuiTextButton BackButton { get; }


    public SettingsScene() {
        BackButton = new GuiTextButton(
            "0.05 0.95 0.125 0.0625",
            "Back",
            new Vector2(0, 1));

        ApplyButton = new GuiTextButton(
            "0.95 0.95 0.125 0.0625",
            "Apply",
            new Vector2(1, 1));

        float xOffset = 0.175f;

        (GuiSelector screenModeSelector, GuiLabel screenModeLabel) = CreateSettingsEntry(
            "Screen Mode", xOffset,
            Enum.GetValues<Settings.eScreenMode>().Select(sm => new GuiSelector.SelectionElement(sm.ToString(), sm)).ToArray(),
            Array.FindIndex(Enum.GetValues<Settings.eScreenMode>(), sm => sm == Application.Settings.ScreenMode));
        ScreenModeLabel = screenModeLabel;
        ScreenModeSelector = screenModeSelector;
        xOffset += 0.1f;

        (GuiSelector resolutionSelector, GuiLabel resolutionLabel) = CreateSettingsEntry(
            "Resolution", xOffset,
            Settings.AVAILABLE_RESOLUTIONS.Select(res => new GuiSelector.SelectionElement($"{res.w}x{res.h}", res)).ToArray(),
            Array.FindIndex<(int, int)>(Settings.AVAILABLE_RESOLUTIONS.ToArray(), r => r.Equals(Application.Settings.GetCurrentResolution())));
        ResolutionLabel = resolutionLabel;
        ResolutionSelector = resolutionSelector;
        xOffset += 0.1f;

        (GuiSelector musicVolumeSelector, GuiLabel musicVolumeLabel) = CreateSettingsEntry(
            "Music Volume", xOffset,
            Enumerable.Range(0, 11).Select(i => new GuiSelector.SelectionElement($"{i * 10f}%", i * 10)).ToArray(),
            Application.Settings.MusicVolume / 10);
        MusicVolumeLabel = musicVolumeLabel;
        MusicVolumeSelector = musicVolumeSelector;
        xOffset += 0.1f;

        (GuiSelector soundVolumeSelector, GuiLabel soundVolumeLabel) = CreateSettingsEntry(
            "Sound Volume", xOffset,
            Enumerable.Range(0, 11).Select(i => new GuiSelector.SelectionElement($"{i * 10f}%", i * 10)).ToArray(),
            Application.Settings.SoundVolume / 10);
        SoundVolumeLabel = soundVolumeLabel;
        SoundVolumeSelector = soundVolumeSelector;
        xOffset += 0.1f;

        ResetTutorialButton = new GuiTextButton("0.5 0.95 0.25 0.0625", "Reset Tutorial", new Vector2(0.5f, 1));
    }

    internal override void Load() {
        LoadAllGuiElements();
    }

    internal override void Render(float dT) {
        BackButton.Draw();
        ApplyButton.Draw();

        ScreenModeLabel.Draw();
        ResolutionLabel.Draw();
        MusicVolumeLabel.Draw();
        SoundVolumeLabel.Draw();

        ScreenModeSelector.Draw();
        ResolutionSelector.Draw();
        MusicVolumeSelector.Draw();
        SoundVolumeSelector.Draw();

        ResetTutorialButton.Draw();

        if (BackButton.IsClicked)
            GameManager.SetScene(new MainMenuScene());
        if (ApplyButton.IsClicked)
            ApplySettings();

        if (ResetTutorialButton.IsClicked)
            Application.Settings.ResetTutorial();
    }

    private void ApplySettings() {
        Settings.eScreenMode screenMode = (Settings.eScreenMode)ScreenModeSelector.SelectedElement.Element;
        (int w, int h) resolution = ((int w, int h))ResolutionSelector.SelectedElement.Element;
        int soundVolume = (int)SoundVolumeSelector.SelectedElement.Element;
        int musicVolume = (int)MusicVolumeSelector.SelectedElement.Element;

        bool needsRestart = false;
        if (resolution != Application.Settings.GetCurrentResolution()) {
            Application.Settings.SetResolution(resolution.w, resolution.h);
            needsRestart = true;
        }
        if (screenMode != Application.Settings.ScreenMode) {
            Application.Settings.SetScreenMode(screenMode);
            needsRestart = true;
        }
        if (soundVolume != Application.Settings.SoundVolume)
            Application.Settings.SoundVolume = soundVolume;
        if (musicVolume != Application.Settings.MusicVolume)
            Application.Settings.MusicVolume = musicVolume;


        if (needsRestart) {
            // This is needed because if the resolution, screen mode or monitor is change the UI is all fricked up
            Application.Exit();
            Process.Start(Environment.ProcessPath, string.Join(" ", Environment.GetCommandLineArgs()));
        }
    }

    private (GuiSelector, GuiLabel) CreateSettingsEntry(string title, float xOffset, GuiSelector.SelectionElement[] selectionElements, int selectedIndex) {
        GuiLabel label = new GuiLabel($"0.135 {xOffset} 0.25 {1f / 10f}", title, new Vector2(0, 0.5f));
        label.TextAlignment = eTextAlignment.Left;
        label.Color = ResourceManager.ColorLoader.Get("font_dark");

        GuiSelector selector = new GuiSelector($"0.35 {xOffset} 0.5 {1f / 16f}",
            selectionElements, selectedIndex < 0 ? 0 : selectedIndex,
            new Vector2(0, 0.5f));

        return (selector, label);
    }
}
