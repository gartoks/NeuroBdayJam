using NeuroBdayJam.Game.Gui;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using System.Numerics;

namespace NeuroBdayJam.Game.Scenes;
internal sealed class MainMenuScene : Scene {
    private GuiImage TitleImage { get; }
    private GuiTextButton PlayButton { get; }
    private GuiTextButton SettingsButton { get; }
    private GuiTextButton QuitButton { get; }


    public MainMenuScene() {

        float xOffset = 0.75f;
        float yOffset = 0.45f;
        PlayButton = new GuiTextButton($"{xOffset} {yOffset} 0.25 0.1", "Play", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;
        SettingsButton = new GuiTextButton($"{xOffset} {yOffset} 0.25 0.1", "Settings", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;
        QuitButton = new GuiTextButton($"{xOffset} {yOffset} 0.25 0.1", "Quit", new Vector2(0.5f, 0.5f));
        yOffset += 0.125f;

        TitleImage = new GuiImage(
            Application.BASE_WIDTH * 0.3f, Application.BASE_HEIGHT * 0.4f,
            1.5f,
            ResourceManager.TextureLoader.Fallback,
            new Vector2(0.5f, 0.5f));
        //TitleImage.Rotation = -MathF.PI / 16f;
    }

    internal override void Load() {
        TextureResource titleTexture = ResourceManager.TextureLoader.Get("title_logo");
        TitleImage.Texture = titleTexture;

        ResourceManager.TextureAtlasLoader.Load("player_animations");

        LoadAllGuiElements();
    }

    internal override void Unload() {
    }

    internal override void Update(float dT) {
    }

    internal override void Render(float dT) {
        PlayButton.Draw();
        SettingsButton.Draw();
        QuitButton.Draw();

        if (PlayButton.IsClicked)
            GameManager.SetScene(new GameScene());
        if (SettingsButton.IsClicked)
            GameManager.SetScene(new SettingsScene());
        if (QuitButton.IsClicked)
            Application.Exit();

        TitleImage.Draw();
    }
}
