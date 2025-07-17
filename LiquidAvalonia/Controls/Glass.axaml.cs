using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LiquidAvalonia.Controls;

public class Glass : ContentControl
{
    /// <summary>
    /// Defines the <see cref="BlurRadius"/> property.
    /// </summary>
    public static readonly StyledProperty<double> BlurRadiusProperty = 
        AvaloniaProperty.Register<GlassImpl, double>(nameof(BlurRadius), defaultValue: 2.0);
    
    /// <summary>
    /// Background blur radius.
    /// </summary>
    public double BlurRadius
    {
        get => GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="TintColor"/> property.
    /// </summary>
    public static readonly StyledProperty<Color> TintColorProperty =
        AvaloniaProperty.Register<GlassImpl, Color>(nameof(TintColor), defaultValue: Colors.White);

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
        AvaloniaProperty.Register<GlassImpl, double>(nameof(TintOpacity), defaultValue: .5);
    
    /// <summary>
    /// Tint opacity.
    /// </summary>
    public double TintOpacity
    {
        get => GetValue(TintOpacityProperty);
        set => SetValue(TintOpacityProperty, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<double> SmoothnessProperty =
        AvaloniaProperty.Register<GlassImpl, double>(nameof(Smoothness), defaultValue: .4);
    
    /// <summary>
    /// 
    /// </summary>
    public double Smoothness
    {
        get => GetValue(SmoothnessProperty);
        set => SetValue(SmoothnessProperty, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<double> RefractionThresholdProperty =
        AvaloniaProperty.Register<GlassImpl, double>(nameof(RefractionThreshold), defaultValue: .4);
    
    /// <summary>
    /// 
    /// </summary>
    public double RefractionThreshold
    {
        get => GetValue(RefractionThresholdProperty);
        set => SetValue(RefractionThresholdProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == TintOpacityProperty && change.NewValue is double opacity)
        {
            DrawShadow(opacity);
        }
    }

    private void DrawShadow(double opacity)
    {
        Effect = new DropShadowEffect
        {
            OffsetX = 0,
            OffsetY = 10,
            BlurRadius = 50,
            Opacity = opacity,
            Color = Colors.Black
        };
    }
}