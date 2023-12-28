using NeuroBDayJam.Util;
using NeuroBDayJam.App;
using Raylib_CsLo;
using System.Numerics;

namespace NeuroBDayJam.Game.Gui;
internal abstract class InteractiveGuiElement : GuiElement {

    internal bool IsHovered => Bounds.Contains(Input.ScreenToWorld(Raylib.GetMousePosition()));

    protected InteractiveGuiElement(float x, float y, float w, float h, Vector2? pivot)
        : base(x, y, w, h, pivot) {
    }

    internal void Focus() {
        GuiManager.Focus(this);
    }

    internal void Unfocus() {
        if (HasFocus())
            GuiManager.Focus(null);
    }

    internal bool HasFocus() {
        return GuiManager.HasFocus(this);
    }
}
