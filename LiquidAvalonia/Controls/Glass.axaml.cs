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
    /// Opacity of tint.
    /// </summary>
    public double TintOpacity
    {
        get => GetValue(TintOpacityProperty);
        set => SetValue(TintOpacityProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="Smoothness"/> property.
    /// </summary>
    public static readonly StyledProperty<double> SmoothnessProperty =
        AvaloniaProperty.Register<GlassImpl, double>(nameof(Smoothness), defaultValue: .4);
    
    /// <summary>
    /// Smoothness of glass shape.
    /// </summary>
    /// <remarks>
    /// Default value is 0.6.
    /// <br/>
    /// Use 0.8 for circular shape.
    /// </remarks>
    public double Smoothness
    {
        get => GetValue(SmoothnessProperty);
        set => SetValue(SmoothnessProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="Depth"/> property.
    /// </summary>
    public static readonly StyledProperty<double> DepthProperty =
        AvaloniaProperty.Register<GlassImpl, double>(nameof(Depth), defaultValue: .6);
    
    /// <summary>
    /// Depth of refraction.
    /// Higher value means more refraction.
    /// </summary>
    /// <remarks>
    /// Default value is 0.6.
    /// </remarks>
    public double Depth
    {
        get => GetValue(DepthProperty);
        set => SetValue(DepthProperty, value);
    }

    public Glass()
    {
        DrawShadow(TintOpacity);
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