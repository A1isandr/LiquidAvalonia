using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace LiquidAvalonia.Controls;

/// <summary>
/// Squircle control.
/// </summary>
[TemplatePart("PART_SquircleImpl", typeof(SquircleImpl))]
public class Squircle : ContentControl
{
    /// <summary>
    /// Defines the <see cref="CornerSmoothing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> CornerSmoothingProperty =
        AvaloniaProperty.Register<Squircle, double>(nameof(CornerSmoothing), defaultValue: 0.6);

    /// <summary>
    /// Corner smoothing.
    /// </summary>
    /// <remarks>
    /// Default value is 0.6 for ios-like effect.
    /// </remarks>
    public double CornerSmoothing
    {
        get => GetValue(CornerSmoothingProperty);
        set => SetValue(CornerSmoothingProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="PreserveSmoothing"/> property.
    /// </summary>
    /// <remarks>
    /// Experimental. May cause artifacts.
    /// </remarks>
    public static readonly StyledProperty<bool> PreserveSmoothingProperty =
        AvaloniaProperty.Register<Squircle, bool>(nameof(PreserveSmoothing), defaultValue: false);

    /// <summary>
    /// Indicate whether corner smoothing should be preserved
    /// even when there is not enough space.
    /// </summary>
    /// <remarks>
    /// Experimental. May cause artifacts when used with high corner smoothing.
    /// Default value is false.
    /// </remarks>
    public bool PreserveSmoothing
    {
        get => GetValue(PreserveSmoothingProperty);
        set => SetValue(PreserveSmoothingProperty, value);
    }
}