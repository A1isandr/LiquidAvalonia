using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;
// ReSharper disable ReplaceWithPrimaryConstructorParameter

namespace LiquidAvalonia.Controls;

internal class BlurImpl : SquircleImpl
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
        AvaloniaProperty.Register<GlassImpl, double>(nameof(TintOpacity), defaultValue: 0.5);
    
    /// <summary>
    /// Opacity of tint.
    /// </summary>
    public double TintOpacity
    {
        get => GetValue(TintOpacityProperty);
        set => SetValue(TintOpacityProperty, value);
    }

    static BlurImpl()
    {
        AffectsRender<BlurImpl>(
            BlurRadiusProperty,
            TintColorProperty,
            TintOpacityProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        context.Custom(new BlurDrawOperation(
            this,
            Bounds,
            BlurRadius,
            TintColor,
            TintOpacity));
    }
    
    private class BlurDrawOperation(
        BlurImpl blur,
        Rect controlBounds,
        double blurRadius,
        Color tintColor,
        double tintOpacity)
        : ICustomDrawOperation
    {
        private readonly BlurImpl _blur = blur;
        private readonly Rect _controlBounds = controlBounds;
        private readonly double _blurRadius = blurRadius;
        private readonly Color _tintColor = tintColor;
        private readonly double _tintOpacity = tintOpacity;
        
        private bool _disposed;

        public Rect Bounds => _controlBounds;
        
        public bool Equals(ICustomDrawOperation? other)
        {
            return other is BlurDrawOperation operation &&
                   operation._blur == _blur &&
                   operation._controlBounds == _controlBounds &&
                   Math.Abs(operation._blurRadius - _blurRadius) < double.Epsilon &&
                   operation._tintColor == _tintColor &&
                   Math.Abs(operation._tintOpacity - _tintOpacity) < double.Epsilon;
        }
    
        public void Dispose()
        {
            if (_disposed)
                return;
            
            _disposed = true;
        }
        
        public bool HitTest(Point p) => _controlBounds.Contains(p);
        
        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null) return;
            
            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            
            if (!canvas.TotalMatrix.TryInvert(out var invertedTransform)) return;
            
            // Preventing artifacts.
            if (canvas.GetLocalClipBounds(out var bounds) && 
                !bounds.Contains(SKRect.Create(
                    bounds.Left,
                    bounds.Top,
                    (float)_controlBounds.Width,
                    (float)_controlBounds.Height)))
            {
                Dispatcher.UIThread.Post(() => _blur.InvalidateVisual());
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
                sigmaX:   (float)_blurRadius,
                sigmaY:   (float)_blurRadius,
                tileMode: SKShaderTileMode.Clamp);

            using var blurPaint = new SKPaint();
            blurPaint.Shader = backgroundShader;
            blurPaint.ImageFilter = blurFilter;
            blurPaint.ColorFilter = SKColorFilter.CreateBlendMode(
                c:    GetEffectiveTintColor(_tintColor, _tintOpacity).ToSKColor(),
                mode: SKBlendMode.Screen);
            
            canvas.DrawRect(0, 0, (float)_controlBounds.Width, (float)_controlBounds.Height, blurPaint);
        }
        
        /// <summary>
        /// Get tint color with opacity.
        /// </summary>
        /// <param name="tintColor"></param>
        /// <param name="tintOpacity"></param>
        /// <returns></returns>
        private static Color GetEffectiveTintColor(Color tintColor, double tintOpacity) =>
            new((byte)(tintColor.A * tintOpacity), tintColor.R, tintColor.G, tintColor.B);
    }
}