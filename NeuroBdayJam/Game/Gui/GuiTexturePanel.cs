using NeuroBdayJam.ResourceHandling.Resources;
using NeuroBdayJam.Util;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Gui;
internal sealed class GuiTexturePanel : GuiElement {

    public GuiPanel Panel { get; }
    public IDrawableResource Texture { get; set; }

    public ColorResource Tint { get; set; }

    public Vector2 TextureScale { get; set; } = Vector2.One;
    private Rectangle TextureBounds => new Rectangle(
        Bounds.X - Bounds.width / 2 + Bounds.width / 2 * TextureScale.X,
        Bounds.Y - Bounds.height / 2 + Bounds.height / 2 * TextureScale.Y,
        Bounds.width * TextureScale.X,
        Bounds.height * TextureScale.Y);

    public GuiTexturePanel(string boundsString, IDrawableResource drawable, Vector2? pivot = null)
        : this(GuiBoundsParser.Parse(boundsString), drawable, pivot) {
    }

    public GuiTexturePanel(Rectangle bounds, IDrawableResource drawable, Vector2? pivot = null)
        : this(bounds.X, bounds.Y, bounds.width, bounds.height, drawable, pivot) {
    }

    public GuiTexturePanel(float x, float y, float w, float h, IDrawableResource drawable, Vector2? pivot = null)
        : base(x, y, w, h, pivot) {

        Panel = new GuiPanel(Bounds, "panel", pivot);

        Texture = drawable;
        Tint = ColorResource.WHITE;
        TextureScale = new Vector2(1, 1);
    }

    internal override void Load() {
        base.Load();
    }

    protected override void DrawInternal() {
        Panel.Draw();
        Texture.Draw(TextureBounds, Pivot + new Vector2(0.5f), 0, Tint.Resource);
    }

}
