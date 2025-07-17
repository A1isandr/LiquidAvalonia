using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace LiquidAvalonia.Controls;

internal class GlassImpl : Control
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
    
    private readonly SKRuntimeEffect _shapeEffect;
    private readonly SKRuntimeEffect _glassEffect;
    
    public GlassImpl()
    {
        const string shapeShaderCode =
            """
            uniform float2 u_resolution;
            uniform float u_smoothness;
            uniform float u_threshold;
            
            
            
            const float size = 0.5;
            
            
            
            float step(float a, float b) {
                if (b < a) {
                    return .0;
                }
                
                return 1.;
            }
            
            float smoothstep(float a, float b, float c) {
                float t = clamp((c - a) / (b - a), .0, 1.);
                
                return t * t * (3. - 2. * t);
            }
            
            float4 main(float2 fragCoord) {
                float2 st = fragCoord / u_resolution;
                float2 point = float2(.5);
            
                float shape = 1. - smoothstep(.0, 1., pow(pow(abs((st.x - point.x) / size), u_smoothness) + pow(abs((st.y - point.y) / size), u_smoothness), u_threshold));
            	
              	// r - refraction intensity.
              	// g - tint area.
            	return float4(shape, smoothstep(0., 0., shape), 0., 1.);
            }
            """;
        
        const string glassShaderCode =
            """
            uniform shader u_shape;
            uniform shader u_background;
            uniform float4 u_tint;
            uniform float u_tint_opacity;
            
            
            
            float smoothstep(float a, float b, float c) {
                float t = clamp((c - a) / (b - a), .0, 1.);
                
                return t * t * (3. - 2. * t);
            }
            
            float4 main(float2 fragCoord) {
                float4 shape_info = sample(u_shape, fragCoord);
              	float4 color = shape_info.r > 0.0
              	    ? sample(u_background, fragCoord * shape_info.r)
              	    : float4(0., 0., 0., 0.);
              
                return mix(color, shape_info.g * u_tint, clamp(shape_info.g, .0, u_tint_opacity)) * smoothstep(0., .2, sin(shape_info.r));
            }
            """;
        
        _shapeEffect = SKRuntimeEffect.Create(shapeShaderCode, out var errors);
        Debug.Assert(_shapeEffect is not null, errors);
        _glassEffect = SKRuntimeEffect.Create(glassShaderCode, out errors);
        Debug.Assert(_glassEffect is not null, errors);
    }
    
    static GlassImpl()
    {
        AffectsRender<GlassImpl>(
            BlurRadiusProperty,
            TintColorProperty,
            TintOpacityProperty,
            SmoothnessProperty,
            RefractionThresholdProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        context.Custom(new GlassDrawOperation(
            this,
            Bounds,
            _shapeEffect,
            _glassEffect,
            TintColor,
            TintOpacity,
            Smoothness,
            RefractionThreshold));
    }

    private class GlassDrawOperation(
        Control glass,
        Rect controlBounds,
        SKRuntimeEffect shapeEffect,
        SKRuntimeEffect glassEffect,
        Color tint,
        double tintOpacity,
        double smoothness,
        double refractionThreshold)
        : ICustomDrawOperation
    {
        private readonly Control _glass = glass;
        private readonly Rect _controlBounds = controlBounds;
        private readonly SKRuntimeEffect _shapeEffect = shapeEffect;
        private readonly SKRuntimeEffect _glassEffect = glassEffect;
        private readonly Color _tint = tint;
        private readonly double _tintOpacity = tintOpacity;
        private readonly double _smoothness = smoothness;
        private readonly double _refractionThreshold = refractionThreshold;
        
        private bool _disposed;

        public Rect Bounds => _controlBounds;
        
        public bool Equals(ICustomDrawOperation? other)
        {
            return other is GlassDrawOperation operation &&
                   operation._glass == _glass &&
                   operation._controlBounds == _controlBounds &&
                   operation._shapeEffect == _shapeEffect &&
                   operation._glassEffect == _glassEffect &&
                   operation._tint == _tint &&
                   Math.Abs(operation._tintOpacity - _tintOpacity) < double.Epsilon &&
                   Math.Abs(operation._smoothness - _smoothness) < double.Epsilon &&
                   Math.Abs(operation._refractionThreshold - _refractionThreshold) < double.Epsilon;
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
            var width = (float)_controlBounds.Width;
            var height = (float)_controlBounds.Height;
            
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null) return;
            
            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            
            if (!canvas.TotalMatrix.TryInvert(out var invertedTransform)) return;
            
            // Prevent artifacts.
            if (canvas.GetLocalClipBounds(out var bounds) && 
                !bounds.Contains(SKRect.Create(
                    bounds.Left,
                    bounds.Top,
                    width,
                    height)))
            {
                Dispatcher.UIThread.Post(() => _glass.InvalidateVisual());
            }
            
            // Take background snapshot. 
            using var backgroundSnapshot = lease.SkSurface?.Snapshot();
            if (backgroundSnapshot is null) return;
            
            // Prepare shape shader.
            var shapeUniforms = new SKRuntimeEffectUniforms(_shapeEffect)
            {
                ["u_resolution"] = new[] { width, height },
                ["u_smoothness"] = new[] { (float)_smoothness * 10 },
                ["u_threshold"]  = new[] { (float)_refractionThreshold * 10 },
            };
            
            using var shapeShader = _shapeEffect.ToShader(true, shapeUniforms);
            if (shapeShader is null) return;
            
            // Prepare background shader.
            using var backgroundShader = SKShader.CreateImage(
                src:         backgroundSnapshot,
                tmx:         SKShaderTileMode.Clamp,
                tmy:         SKShaderTileMode.Clamp,
                localMatrix: invertedTransform);
            
            // Prepare glass shader.
            var glassUniforms = new SKRuntimeEffectUniforms(_glassEffect)
            {
                ["u_tint"]         = new[] { ToFloat4(_tint) },
                ["u_tint_opacity"] = new[] { (float)_tintOpacity }
            };

            var glassChildren = new SKRuntimeEffectChildren(_glassEffect)
            {
                ["u_shape"]      = shapeShader,
                ["u_background"] = backgroundShader
            };
            
            using var glassShader = _glassEffect.ToShader(true, glassUniforms, glassChildren);
            if (glassShader is null) return;
            
            // Draw glass.
            using var glassPaint = new SKPaint();
            glassPaint.Shader = glassShader;
            glassPaint.IsAntialias = true;
            
            canvas.DrawRect(0, 0, (float)_controlBounds.Width, (float)_controlBounds.Height, glassPaint);
        }

        private static float[] ToFloat4(Color color)
        {
            return [color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f];
        }
    }
}

