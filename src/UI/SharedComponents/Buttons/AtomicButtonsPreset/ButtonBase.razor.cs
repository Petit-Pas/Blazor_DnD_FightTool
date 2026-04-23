using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public partial class ButtonBase
{
    public ButtonBase(Color color, string icon, bool rotateIcon)
    {
        Color = color;
        Icon = icon;
        RotateIcon = rotateIcon;
    }

    [Parameter]
    public virtual bool Disabled { get; set; } = false;

    [Parameter]
    public virtual Variant Variant { get; set; } = Variant.Filled;

    [Parameter]
    public virtual EventCallback OnClick { get; set; }

    /// <summary>
    ///     Large will be a rectangle button
    ///     Medium will be a big round button
    ///     Small will be a small round button
    /// </summary>
    [Parameter]
    public virtual Size Size { get; set; } = Size.Large;

    [Parameter]
    public virtual Color Color { get; init; } = Color.Primary;

    /// <summary>
    ///     Will only be used with Size Large
    /// </summary>
    [Parameter]
    public virtual string? Label { get; set; }

    internal virtual string Icon { get; init; } = MudBlazor.Icons.Material.Filled.QuestionMark;

    internal virtual bool RotateIcon { get; init; } = false;
}
