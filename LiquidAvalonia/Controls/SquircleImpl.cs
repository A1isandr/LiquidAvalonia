using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace LiquidAvalonia.Controls;

/// <summary>
/// Squircle implementation.
/// </summary>
internal class SquircleImpl : Control
{
    private record CornerProperties(
        double A,
        double B,
        double C,
        double D,
        double P)
    {
        /// <summary>
        /// Get sum of A and B.
        /// </summary>
        public double Ab { get; } = A + B;
        
        /// <summary>
        /// Get sum of B and C.
        /// </summary>
        public double Bc { get; } = B + C;
        
        /// <summary>
        /// Get sum of A, B and C.
        /// </summary>
        public double Abc { get; } = A + B + C;
        
        /// <summary>
        /// Get subtraction sum of A, B and C from P.
        /// </summary>
        public double PMinusAbc { get; } = P - A - B - C;
    }
    
    /// <summary>
    /// Defines the <see cref="CornerRadius"/> property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<SquircleImpl, CornerRadius>(nameof(CornerRadius), defaultValue: new CornerRadius(20));
    
    /// <summary>
    /// Corner radius.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    } 
    
    /// <summary>
    /// Defines the <see cref="CornerSmoothing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> CornerSmoothingProperty =
        AvaloniaProperty.Register<SquircleImpl, double>(nameof(CornerSmoothing), defaultValue: 0.6);

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
    public static readonly StyledProperty<bool> PreserveSmoothingProperty =
        AvaloniaProperty.Register<SquircleImpl, bool>(nameof(PreserveSmoothing), defaultValue: false);

    /// <summary>
    /// Indicate whether corner smoothing should be preserved
    /// even when there is not enough space.
    /// </summary>
    /// <remarks>
    /// Default value is false.
    /// </remarks>
    public bool PreserveSmoothing
    {
        get => GetValue(PreserveSmoothingProperty);
        set => SetValue(PreserveSmoothingProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="BorderThickness"/> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.Register<SquircleImpl, Thickness>(nameof(BorderThickness), defaultValue: new Thickness(0));
    
    /// <summary>
    /// Border thickness.
    /// </summary>
    public Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="BorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<SquircleImpl, IBrush?>(nameof(BorderBrush), defaultValue: null);
    
    /// <summary>
    /// Border brush.
    /// </summary>
    public IBrush? BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="Background"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<SquircleImpl, IBrush?>(nameof(Background), defaultValue: null);
    
    /// <summary>
    /// Background brush.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    } 
    
    /// <summary>
    /// Defines the <see cref="SquirclePath"/> property.
    /// </summary>
    protected static readonly DirectProperty<SquircleImpl, string?> SquirclePathProperty =
        AvaloniaProperty.RegisterDirect<SquircleImpl, string?>(nameof(SquirclePath),
            o => o.SquirclePath,
            (o, v) => o.SquirclePath = v);
    
    private string? _squirclePath;
    /// <summary>
    /// Path data of the squircle.
    /// </summary>
    protected string? SquirclePath
    {
        get => _squirclePath;
        set => SetAndRaise(SquirclePathProperty, ref _squirclePath, value);
    }
    
    static SquircleImpl()
    {
        AffectsRender<SquircleImpl>(
            CornerRadiusProperty,
            CornerSmoothingProperty,
            PreserveSmoothingProperty,
            BorderThicknessProperty,
            BorderBrushProperty,
            BackgroundProperty);
    }

    // Figma's squircle implementation:
    // https://figma.com/blog/desperately-seeking-squircles/
    // https://github.com/phamfoo/figma-squircle/
    // https://github.com/MartinRGB/Figma_Squircles_Approximation
    public override void Render(DrawingContext context)
    {
        // Prevent invalid path data if height or width is less than or equal to zero.
        if (Bounds.Height <= 0 || Bounds.Width <= 0)
        {
            base.Render(context);
            
            return;
        }
        
        var rect = Bounds;
        var maxRoundingAndSmoothing = Math.Min(Bounds.Width, Bounds.Height) / 2;
        var cornerRadius = new CornerRadius(
            Math.Min(maxRoundingAndSmoothing, CornerRadius.TopLeft),
            Math.Min(maxRoundingAndSmoothing, CornerRadius.TopRight),
            Math.Min(maxRoundingAndSmoothing, CornerRadius.BottomRight),
            Math.Min(maxRoundingAndSmoothing, CornerRadius.BottomLeft));
        
        // Top right corner.
        var topRightProps = GetCornerProperties(
            cornerRadius.TopRight,
            CornerSmoothing,
            PreserveSmoothing,
            maxRoundingAndSmoothing);
        
        // Bottom right corner.
        var bottomRightProps = GetCornerProperties(
            cornerRadius.BottomRight,
            CornerSmoothing,
            PreserveSmoothing,
            maxRoundingAndSmoothing);
        
        // Bottom left corner.
        var bottomLeftProps = GetCornerProperties(
            cornerRadius.BottomLeft,
            CornerSmoothing,
            PreserveSmoothing,
            maxRoundingAndSmoothing);
        
        // Top left corner.
        var topLeftProps = GetCornerProperties(
            cornerRadius.TopLeft,
            CornerSmoothing,
            PreserveSmoothing,
            maxRoundingAndSmoothing);
        
        // Always use "." as decimal separator.
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        var sb = new StringBuilder($"M {rect.Left + topLeftProps.P} {rect.Top} ");
         
        // Top right corner.
        var p1 = new Point(rect.Right - topRightProps.P, rect.Top);
        var p2 = new Point(p1.X + topRightProps.A, rect.Top);
        var p3 = new Point(p1.X + topRightProps.Ab, rect.Top);
        var p4 = new Point(p1.X + topRightProps.Abc, rect.Top + topRightProps.D);
        var p5 = new Point(rect.Right - topRightProps.D, rect.Top + topRightProps.PMinusAbc);
        var p6 = new Point(rect.Right, p5.Y + topRightProps.C);
        var p7 = new Point(rect.Right, p5.Y + topRightProps.Bc);
        var p8 = new Point(rect.Right, p5.Y + topRightProps.Abc);

        sb.Append($"L {p1.X} {p1.Y} ");
        sb.Append($"C {p2.X} {p2.Y} {p3.X} {p3.Y} {p4.X} {p4.Y} ");
        sb.Append($"A {cornerRadius.TopRight} {cornerRadius.TopRight} 0 0 1 {p5.X} {p5.Y} ");
        sb.Append($"C {p6.X} {p6.Y} {p7.X} {p7.Y} {p8.X} {p8.Y} ");
        
        // Bottom right corner.
        p1 = new Point(rect.Right, rect.Bottom - bottomRightProps.P);
        p2 = new Point(rect.Right, p1.Y + bottomRightProps.A);
        p3 = new Point(rect.Right, p1.Y + bottomRightProps.Ab);
        p4 = new Point(rect.Right - bottomRightProps.D, p1.Y + bottomRightProps.Abc);
        p5 = new Point(rect.Right - bottomRightProps.PMinusAbc, rect.Bottom - bottomRightProps.D);
        p6 = new Point(p5.X - bottomRightProps.C, rect.Bottom);
        p7 = new Point(p5.X - bottomRightProps.Bc, rect.Bottom);
        p8 = new Point(p5.X - bottomRightProps.Abc, rect.Bottom);
         
        sb.Append($"L {p1.X} {p1.Y} ");
        sb.Append($"C {p2.X} {p2.Y} {p3.X} {p3.Y} {p4.X} {p4.Y} ");
        sb.Append($"A {cornerRadius.BottomRight} {cornerRadius.BottomRight} 0 0 1 {p5.X} {p5.Y} ");
        sb.Append($"C {p6.X} {p6.Y} {p7.X} {p7.Y} {p8.X} {p8.Y} ");
        
        // Bottom left corner.
        p1 = new Point(rect.Left + bottomLeftProps.P, rect.Bottom);
        p2 = new Point(p1.X - bottomLeftProps.A, rect.Bottom);
        p3 = new Point(p1.X - bottomLeftProps.Ab, rect.Bottom);
        p4 = new Point(p1.X - bottomLeftProps.Abc, rect.Bottom - bottomLeftProps.D);
        p5 = new Point(rect.Left + bottomLeftProps.D, rect.Bottom - bottomLeftProps.PMinusAbc);
        p6 = new Point(rect.Left, p5.Y - bottomLeftProps.C);
        p7 = new Point(rect.Left, p5.Y - bottomLeftProps.Bc);
        p8 = new Point(rect.Left, p5.Y - bottomLeftProps.Abc);
         
        sb.Append($"L {p1.X} {p1.Y} ");
        sb.Append($"C {p2.X} {p2.Y} {p3.X} {p3.Y} {p4.X} {p4.Y} ");
        sb.Append($"A {cornerRadius.BottomLeft} {cornerRadius.BottomLeft} 0 0 1 {p5.X} {p5.Y} ");
        sb.Append($"C {p6.X} {p6.Y} {p7.X} {p7.Y} {p8.X} {p8.Y} ");
        
        // Top left corner.
        p1 = new Point(rect.Left, rect.Top + topLeftProps.P);
        p2 = new Point(rect.Left, p1.Y - topLeftProps.A);
        p3 = new Point(rect.Left, p1.Y - topLeftProps.Ab);
        p4 = new Point(rect.Left + topLeftProps.D, p1.Y - topLeftProps.Abc);
        p5 = new Point(rect.Left + topLeftProps.PMinusAbc, rect.Top + topLeftProps.D);
        p6 = new Point(p5.X + topLeftProps.C, rect.Top);
        p7 = new Point(p5.X + topLeftProps.Bc, rect.Top);
        p8 = new Point(p5.X + topLeftProps.Abc, rect.Top);
        
        sb.Append($"L {p1.X} {p1.Y} ");
        sb.Append($"C {p2.X} {p2.Y} {p3.X} {p3.Y} {p4.X} {p4.Y} ");
        sb.Append($"A {cornerRadius.TopLeft} {cornerRadius.TopLeft} 0 0 1 {p5.X} {p5.Y} ");
        sb.Append($"C {p6.X} {p6.Y} {p7.X} {p7.Y} {p8.X} {p8.Y} ");
        
        SquirclePath = sb.ToString();
        
        // Draw clip path.
        context.Custom(new SquircleDrawOperation(
           SquirclePath,
           Bounds));
        
        // Draw background and border.
        context.DrawGeometry(
            Background,
            new Pen(BorderBrush, BorderThickness.Top),
            Geometry.Parse(SquirclePath));
        
        base.Render(context);
    }
    
    private static CornerProperties GetCornerProperties(
        double cornerRadius,
        double cornerSmoothing,
        bool preserveSmoothing,
        double maxRoundingAndSmoothing)
    {
        // In case of rectangle q = R = 90 deg (Figures 12.1 - 12.2).
        // p = (1 + cornerSmoothing) * q;
        var p = (1 + cornerSmoothing) * cornerRadius;

        if (!preserveSmoothing)
        {
            var maxCornerSmoothing = maxRoundingAndSmoothing / cornerRadius - 1;
            cornerSmoothing = Math.Min(cornerSmoothing, maxCornerSmoothing);
            p = Math.Min(p, maxRoundingAndSmoothing);
        }

        // In case of rounded rectangle this is always 90 deg.
        var arcSweep = 90 * (1 - cornerSmoothing);
        var arcSectionLength = Math.Sin(ToRadians(arcSweep / 2)) * cornerRadius * Math.Sqrt(2);

        // Distance between point P3 and P4 (Figure 11.1).
        var angleAlpha = (90 - arcSweep) / 2;
        var p3ToP4Distance = cornerRadius * Math.Tan(ToRadians(angleAlpha / 2));

        // Find a, b, c, d (Figure 11.1).
        var angleBeta = 45 * cornerSmoothing;
        
        var c = p3ToP4Distance * Math.Cos(ToRadians(angleBeta));
        var d = c * Math.Tan(ToRadians(angleBeta));

        var b = (p - arcSectionLength - c - d) / 3;
        var a = 2 * b;
        
        return new CornerProperties(a, b, c, d, p);
    }
    
    /// <summary>
    /// Convert degrees to radians.
    /// </summary>
    private static double ToRadians(double degrees) =>
        degrees * Math.PI / 180;

    private class SquircleDrawOperation(
        string squirclePath,
        Rect controlBounds)
        : ICustomDrawOperation
    {
        private readonly Rect _controlBounds = controlBounds;
        private readonly string _squirclePath = squirclePath;
        
        private bool _disposed;
    
        public Rect Bounds => _controlBounds;
        
        public bool Equals(ICustomDrawOperation? other)
        {
            return other is SquircleDrawOperation operation &&
                   operation._controlBounds == _controlBounds &&
                   operation._squirclePath == _squirclePath;
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
            
            //canvas.ClipPath(SKPath.ParseSvgPathData(_squirclePath), SKClipOperation.Intersect, true);
        }
    }
}