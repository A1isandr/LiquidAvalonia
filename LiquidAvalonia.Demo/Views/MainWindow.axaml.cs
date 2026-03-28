using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI.Avalonia;
using LiquidAvalonia.Demo.ViewModels;

namespace LiquidAvalonia.Demo.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private bool IsDragging { get; set; }

    private Point DragOrigin { get; set; }

    private Point Start { get; set; }

    private double X { get; set; }

    private double Y { get; set; }



    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainWindowViewModel();

        X = Canvas.GetLeft(LiquidGlass);
        Y = Canvas.GetTop(LiquidGlass);



        LiquidGlass.PointerPressed += (_, e) =>
        {
            if (!e.Properties.IsLeftButtonPressed) return;

            IsDragging = true;
            DragOrigin = e.GetPosition(GlassCanvas);
            Start      = new Point(X, Y);

            e.Pointer.Capture(LiquidGlass);
            LiquidGlass.Cursor = new Cursor(StandardCursorType.SizeAll);
        };

        LiquidGlass.PointerMoved += (_, e) =>
        {
            if (IsDragging)
            {
                var point  = e.GetPosition(GlassCanvas);
                var deltaX = point.X - DragOrigin.X;
                var deltaY = point.Y - DragOrigin.Y;

                X = Start.X + deltaX;
                Y = Start.Y + deltaY;

                Canvas.SetLeft(LiquidGlass, X);
                Canvas.SetTop(LiquidGlass, Y);
            }

        };

        LiquidGlass.PointerReleased += (_, e) =>
        {
            EndDrag(e.Pointer);
        };

        LiquidGlass.PointerCaptureLost += (_, e) =>
        {
            EndDrag(e.Pointer);
        };
    }



    private void EndDrag(IPointer pointer)
    {
        if (!IsDragging) return;

        IsDragging = false;

        pointer.Capture(null);
        LiquidGlass.Cursor = new Cursor(StandardCursorType.Hand);
    }

}
