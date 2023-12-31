using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Diagnostics;
using System.Numerics;

namespace NeuroBdayJam.Graphics;
/// <summary>
/// Class for settup up and controlling the drawing of the game.
/// </summary>
internal static class Renderer {

    /// <summary>
    /// Stopwatch to keep track of the time between frames (delta time).
    /// </summary>
    private static Stopwatch UpdateStopwatch { get; }

    /// <summary>
    /// The default font for buttons and ingame text
    /// </summary>
    internal static FontResource MainFont { get; private set; }
    /// <summary>
    /// The default font to use for the ui.
    /// </summary>
    internal static FontResource GuiFont { get; private set; }

    /// <summary>
    /// Keeps trakc of the time since the game started.
    /// </summary>
    internal static float Time { get; private set; }

    private static List<RenderTexture> RenderTargets { get; } = new List<RenderTexture>();
    public static List<ShaderResource> PostProcessShaders { get; } = new List<ShaderResource>();

    /// <summary>
    /// Static constructor to initialize clear color and required properties.
    /// </summary>
    static Renderer() {
        UpdateStopwatch = new Stopwatch();
        Time = 0;
    }

    /// <summary>
    /// Initializes the drawing. Currently does nothing.
    /// </summary>
    internal static void Initialize() {
    }

    /// <summary>
    /// Loads global resources.
    /// </summary>
    internal static void Load() {
        MainFont = ResourceManager.FontLoader.Get("main");
        GuiFont = ResourceManager.FontLoader.Get("gui");
    }

    /*private static void StartPostProcessRendering() {
        Raylib.BeginTextureMode(RenderTarget);
        Raylib.ClearBackground(Raylib.RAYWHITE);

        Raylib.DrawCircle(200, 200, 400, Raylib.RED);
    }

    private static void EndPostProcessRendering() {
        if (RenderTarget.id == 0)
            RenderTarget = Raylib.LoadRenderTexture(Application.BASE_WIDTH, Application.BASE_HEIGHT);

        Raylib.EndTextureMode();

        Raylib.BeginDrawing();
        Raylib.BeginShaderMode(PostProcessShader!.Value);
        Raylib.DrawTextureRec(RenderTarget.texture, new Rectangle(0, 0, RenderTarget.texture.width, -RenderTarget.texture.height), new Vector2(0, 0), Raylib.WHITE);
        Raylib.EndShaderMode();

        PostProcessShader = null;
    }*/

    /// <summary>
    /// Main drawing method. Called every frame. Tracks delta time and calls the game's draw method. Also scales all drawing operations to the game's resolution.
    /// </summary>
    internal static void Render() {
        UpdateStopwatch.Stop();
        long ms = UpdateStopwatch.ElapsedMilliseconds;
        float dT = ms / 1000f;
        Time += dT;
        UpdateStopwatch.Restart();

        RlGl.rlPushMatrix();
        RlGl.rlScalef(Application.WorldToScreenMultiplierX, Application.WorldToScreenMultiplierY, 1);

        if (PostProcessShaders.Count > 0) {
            while (RenderTargets.Count < PostProcessShaders.Count)
                RenderTargets.Add(Raylib.LoadRenderTexture(Application.BASE_WIDTH, Application.BASE_HEIGHT));

            float time = Time;
            for (int i = 0; i < PostProcessShaders.Count; i++) {
                ShaderResource shader = PostProcessShaders[i];

                if (!shader.IsLoaded)
                    shader.WaitForLoad();

                RenderTexture renderTarget = RenderTargets[i];

                int timeLoc = Raylib.GetShaderLocation(shader.Resource, "time");
                Raylib.SetShaderValue(shader.Resource, timeLoc, time, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

                RenderTexture? prevRenderTarget = i == 0 ? null : RenderTargets[i - 1];
                ShaderResource? prevShader = i == 0 ? null : PostProcessShaders[i - 1];

                Raylib.BeginTextureMode(renderTarget);
                Raylib.ClearBackground(new Color(0, 0, 0, 0));
                Game.GameManager.RenderPostProcessed(shader, dT);

                if (prevRenderTarget.HasValue) {
                    Raylib.BeginShaderMode(prevShader!.Resource);
                    Raylib.DrawTextureRec(prevRenderTarget.Value.texture, new Rectangle(0, 0, prevRenderTarget.Value.texture.width, -prevRenderTarget.Value.texture.height), new Vector2(0, 0), Raylib.WHITE);
                    Raylib.EndShaderMode();
                }

                Raylib.EndTextureMode();
            }
        }

        Raylib.BeginDrawing();
        Raylib.ClearBackground(ResourceManager.ColorLoader.Get("background").Resource);

        if (PostProcessShaders.Count > 0) {
            ShaderResource shader = PostProcessShaders[^1];
            RenderTexture renderTarget = RenderTargets[^1];

            Raylib.BeginShaderMode(shader.Resource);
            Raylib.DrawTextureRec(renderTarget.texture, new Rectangle(0, 0, renderTarget.texture.width, -renderTarget.texture.height), new Vector2(0, 0), Raylib.WHITE);
            Raylib.EndShaderMode();
        }
        /*for (int i = 0; i < PostProcessShaders.Count; i++) {
            ShaderResource shader = PostProcessShaders[i];
            RenderTexture renderTarget = RenderTargets[i];

            Raylib.BeginShaderMode(shader.Resource);
            Raylib.DrawTextureRec(renderTarget.texture, new Rectangle(0, 0, renderTarget.texture.width, -renderTarget.texture.height), new Vector2(0, 0), Raylib.WHITE);
            Raylib.EndShaderMode();
        }*/

        Game.GameManager.Render(dT);

        RlGl.rlPopMatrix();

        if (Application.DRAW_DEBUG) {
            int fps = Raylib.GetFPS();
            Raylib.DrawText(fps.ToString(), 10, 10, 16, Raylib.LIME);

            float x = Raylib.GetMouseX() / (float)Raylib.GetRenderWidth();
            float y = Raylib.GetMouseY() / (float)Raylib.GetRenderHeight();
            Raylib.DrawText($"{Raylib.GetMousePosition()}, ({x:0.000}, {y:0.000})", 30, 30, 16, Raylib.MAGENTA);
        }

        Raylib.EndDrawing();
    }
}
