﻿using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using System.Numerics;

namespace NeuroBdayJam.Game.Scenes;
internal sealed class MainMenuScene : Scene {
    private GUIImage TitleImage { get; }
    private GuiTextButton StartButton { get; }
    private GuiTextButton LoadButton { get; }
    private GuiTextButton SettingsButton { get; }
    private GuiTextButton QuitButton { get; }


    public MainMenuScene() {

        float yOffset = 0.4f;
        StartButton = new GuiTextButton($"0.5 {yOffset} 0.25 0.1", "Start", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;
        LoadButton = new GuiTextButton($"0.5 {yOffset} 0.25 0.1", "Load", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;
        SettingsButton = new GuiTextButton($"0.5 {yOffset} 0.25 0.1", "Settings", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;
        QuitButton = new GuiTextButton($"0.5 {yOffset} 0.25 0.1", "Quit", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;

        TitleImage = new GUIImage(
            Application.BASE_WIDTH / 2f, Application.BASE_HEIGHT * 0.05f,
            0.5f,
            ResourceManager.TextureLoader.Fallback,
            new Vector2(0.5f, 0));
    }

    internal override void Load() {
        TextureResource titleTexture = ResourceManager.TextureLoader.Get("title_logo");
        TitleImage.Texture = titleTexture;



        LoadAllGuiElements();
    }

    internal override void Unload() {
    }

    internal override void Update(float dT) {
    }

    internal override void Draw(float dT) {
        StartButton.Draw();
        SettingsButton.Draw();
        QuitButton.Draw();

        if (StartButton.IsClicked)
            GameManager.SetScene(new WorldGenTestScene());
        if (LoadButton.IsClicked)
            GameManager.SetScene(new LoadSaveScene());
        if (SettingsButton.IsClicked)
            GameManager.SetScene(new SettingsScene());
        if (QuitButton.IsClicked)
            Application.Exit();

        TitleImage.Draw();
    }
}
