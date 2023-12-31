using NeuroBdayJam.ResourceHandling.Resources;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBdayJam.Game.Gui;
internal sealed class GUIImage : GuiElement {
    public IDrawableResource Texture { get; set; }
    public ColorResource Tint { get; set; }

    public float Rotation { get; set; }
    public float Scale { get; set; }

    public GUIImage(float x, float y, float scale, IDrawableResource texture, Vector2? pivot = null)
        : base(x, y, scale * texture.Width, scale * texture.Height, pivot) {

        Scale = scale;
        Texture = texture;
        Tint = ColorResource.WHITE;
    }

    protected override void DrawInternal() {
        Texture.Draw(new Rectangle(Bounds.x, Bounds.y, Scale * Texture.Width, Scale * Texture.Height), Pivot, Rotation, Tint.Resource);
        //Texture.Draw(new Vector2(Bounds.x, Bounds.y), Pivot, new Vector2(Scale, Scale), Rotation, Tint.Resource);

        //Raylib.DrawTexturePro(
        //    Texture.Resource,
        //        new Rectangle(0, 0, Texture.Resource.width, Texture.Resource.height),
        //        Bounds,
        //        Pivot,
        //        0,
        //        Tint.Resource);
    }

}
