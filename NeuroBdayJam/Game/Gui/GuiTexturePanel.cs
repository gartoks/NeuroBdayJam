using NeuroBdayJam.Util;
using NeuroBdayJam.ResourceHandling;
using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Gui;
internal sealed class GuiTexturePanel : GuiElement {

    private GuiPanel Panel;
    private string _TextureKey { get; set; }
    public string TextureKey {
        get => _TextureKey;
        set {
            _TextureKey = value;
            Texture = null;
        }
    }
    private TextureResource? Texture { get; set; }

    public ColorResource Tint { get; set; }

    public Vector2 TextureScale { get; set; } = Vector2.One;
    private Rectangle TextureBounds => new Rectangle(
        Bounds.X - Bounds.width/2 + Bounds.width/2 * TextureScale.X,
        Bounds.Y - Bounds.height/2 + Bounds.height/2 * TextureScale.Y,
        Bounds.width * TextureScale.X,
        Bounds.height * TextureScale.Y);

    public GuiTexturePanel(string boundsString, string textureKey, Vector2? pivot = null)
        : this(GuiBoundsParser.Parse(boundsString), textureKey, pivot) {
    }

    public GuiTexturePanel(Rectangle bounds, string textureKey, Vector2? pivot = null)
        : this(bounds.X, bounds.Y, bounds.width, bounds.height, textureKey, pivot) {
        }

    public GuiTexturePanel(float x, float y, float w, float h, string textureKey, Vector2? pivot = null)
        : base(x, y, w, h, pivot) {

        Panel = new GuiPanel(Bounds, "panel", pivot);
        
        TextureKey = textureKey;
        Tint = ColorResource.WHITE;
        TextureScale = new Vector2(1, 1);
    }

    internal override void Load() {
        base.Load();

        Texture = ResourceManager.TextureLoader.Get(TextureKey);
    }

    protected override void DrawInternal() {
        if (Texture == null)
            Texture = ResourceManager.TextureLoader.Get(TextureKey);

        Panel.Draw();
        Texture.Draw(TextureBounds, Pivot*1.5f, 0, Tint.Resource);
    }

}
