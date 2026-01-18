using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Media;

namespace LiquidAvalonia.Controls;

/// <summary>
/// 
/// </summary>
[TemplatePart("PART_BlurImpl", typeof(BlurImpl))]
public class Blur : Squircle
{
    /// <summary>
    /// Defines the <see cref="BlurRadius"/> property.
    /// </summary>
    public static readonly StyledProperty<double> BlurRadiusProperty = 
        AvaloniaProperty.Register<BlurImpl, double>(nameof(BlurRadius), defaultValue: 10.0);
    
    /// <summary>
    /// Background blur radius.
    /// </summary>
    /// <remarks>
    /// Default value is 10.
    /// </remarks>
    public double BlurRadius
    {
        get => GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="TintColor"/> property.
    /// </summary>
    public static readonly StyledProperty<Color> TintColorProperty =
        AvaloniaProperty.Register<BlurImpl, Color>(nameof(TintColor), defaultValue: Colors.Transparent);

    /// <summary>
    /// Color of tint.
    /// </summary>
    public Color TintColor
    {
        get => GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="TintOpacity"/> property.
    /// </summary>
    public static readonly StyledProperty<double> TintOpacityProperty =
        AvaloniaProperty.Register<BlurImpl, double>(nameof(TintOpacity), defaultValue: 0.5);
    
    /// <summary>
    /// Opacity of tint.
    /// </summary>
    public double TintOpacity
    {
        get => GetValue(TintOpacityProperty);
        set => SetValue(TintOpacityProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="TintBlendMode"/> property.
    /// </summary>
    public static readonly StyledProperty<BlendMode> TintBlendModeProperty =
        AvaloniaProperty.Register<BlurImpl, BlendMode>(nameof(TintBlendMode), defaultValue: BlendMode.Screen);

    /// <summary>
    /// Blend mode.
    /// </summary>
    /// <remarks>Default value is Screen.</remarks>
    public BlendMode TintBlendMode
    {
        get => GetValue(TintBlendModeProperty);
        set => SetValue(TintBlendModeProperty, value);
    }
}



/// <summary>
/// Tint color's blend mode.
/// </summary>
public enum BlendMode
{
    Screen,
    Color,
    Overlay,
    Hue,
    Saturation
}
