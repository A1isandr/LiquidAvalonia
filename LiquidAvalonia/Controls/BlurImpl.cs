using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;
// ReSharper disable ReplaceWithPrimaryConstructorParameter

namespace LiquidAvalonia.Controls;

internal class BlurImpl : ContentControl
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
        AvaloniaProperty.Register<GlassImpl, Color>(nameof(TintColor), defaultValue: Colors.Transparent);

    /// <summary>
    /// Tint's color.
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
        AvaloniaProperty.Register<GlassImpl, double>(nameof(TintOpacity), defaultValue: 0.5);
    
    /// <summary>
    /// Tint's opacity.
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
    /// Tint's blend mode.
    /// </summary>
    /// <remarks>Default value is Screen.</remarks>
    public BlendMode TintBlendMode
    {
        get => GetValue(TintBlendModeProperty);
        set => SetValue(TintBlendModeProperty, value);
    }



    static BlurImpl()
    {
        AffectsRender<BlurImpl>(
            BlurRadiusProperty,
            TintColorProperty,
            TintOpacityProperty,
            TintBlendModeProperty);
    }

   

    public override void Render(DrawingContext context)
    {
        context.Custom(new BlurDrawOperation(new BlurDrawOperationArgs(
            this,
            Bounds,
            BlurRadius,
            TintColor,
            TintOpacity,
            TintBlendMode)));
        
        base.Render(context);
    }

   
   
    private class BlurDrawOperation(BlurDrawOperationArgs args) : ICustomDrawOperation
    {
        private readonly BlurDrawOperationArgs _args = args;
        
        private bool _disposed;
        
        public Rect Bounds => _args.BlurBounds;



        public bool Equals(ICustomDrawOperation? other)
        {
            return other is BlurDrawOperation operation &&
                   _args.Equals(operation._args);
        }
    
        public void Dispose()
        {
            if (_disposed)
                return;
            
            _disposed = true;
        }
        
        public bool HitTest(Point p) => _args.BlurBounds.Contains(p);
        
        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null) return;
            
            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            
            if (!canvas.TotalMatrix.TryInvert(out var invertedTransform)) return;
            
            var width  = (float)_args.BlurBounds.Width;
            var height = (float)_args.BlurBounds.Height;
            
            // Preventing artifacts.
            if (canvas.GetLocalClipBounds(out var bounds) && 
                !bounds.Contains(SKRect.Create(
                    bounds.Left,
                    bounds.Top,
                    width,
                    height)))
            {
                Dispatcher.UIThread.Post(() => _args.Blur.InvalidateVisual());
            }
            
            // Take background snapshot. 
            using var backgroundSnapshot = lease.SkSurface?.Snapshot();
            if (backgroundSnapshot is null) return;
            
            using var backgroundShader = SKShader.CreateImage(
                src:         backgroundSnapshot,
                tmx:         SKShaderTileMode.Clamp,
                tmy:         SKShaderTileMode.Clamp,
                localMatrix: invertedTransform);
            
            // Create blur.
            using var blurFilter = SKImageFilter.CreateBlur(
                sigmaX:   (float)_args.BlurRadius,
                sigmaY:   (float)_args.BlurRadius,
                tileMode: SKShaderTileMode.Clamp);

            using var blurPaint = new SKPaint();
            blurPaint.Shader      = backgroundShader;
            blurPaint.ImageFilter = blurFilter;
            blurPaint.ColorFilter = SKColorFilter.CreateBlendMode(
                c:    GetEffectiveTintColor(_args.TintColor, _args.TintOpacity).ToSKColor(),
                mode: BlendModeToSKBlendMode(_args.TintBlendMode));
            
            canvas.DrawRect(0, 0, width, height, blurPaint);
        }
        
        /// <summary>
        /// Apply opacity to tint color.
        /// </summary>
        /// <param name="tintColor">Tint's color.</param>
        /// <param name="tintOpacity">Tint's opacity.</param>
        /// <returns>Effective tint color.</returns>
        private static Color GetEffectiveTintColor(Color tintColor, double tintOpacity) =>
            new((byte)(tintColor.A * tintOpacity), tintColor.R, tintColor.G, tintColor.B);

        /// <summary>
        /// Convert BlendMode to SKBlendMode.
        /// </summary>
        /// <param name="blendMode">Tint's blend mode.</param>
        /// <returns>Skia blend mode.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Unknown blend mode.</exception>
        // ReSharper disable once InconsistentNaming
        private static SKBlendMode BlendModeToSKBlendMode(BlendMode blendMode)
        {
            return blendMode switch
            {
                BlendMode.Screen => SKBlendMode.Screen,
                BlendMode.Color => SKBlendMode.Color,
                BlendMode.Overlay => SKBlendMode.Overlay,
                BlendMode.Hue => SKBlendMode.Hue,
                BlendMode.Saturation => SKBlendMode.Saturation,
                _ => throw new ArgumentOutOfRangeException(nameof(blendMode), blendMode, null)
            };
        }
    }
}



internal record BlurDrawOperationArgs(
    BlurImpl  Blur,
    Rect      BlurBounds,
    double    BlurRadius,
    Color     TintColor,
    double    TintOpacity,
    BlendMode TintBlendMode);