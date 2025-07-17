using Avalonia.Media;
using Avalonia.ReactiveUI;
using LiquidAvalonia.Controls;
using LiquidAvalonia.Demo.ViewModels;

namespace LiquidAvalonia.Demo.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private bool _isPointerPressed;
    
    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainWindowViewModel();
        
        // GlassPanel.PointerPressed += (sender, e) =>
        // {
        //     if (sender is not Glass _) return;
        //
        //     //e.Pointer.Capture(Glass);
        //     _isPointerPressed = true;
        // };
        //
        // GlassPanel.PointerReleased += (sender, e) =>
        // {
        //     if (sender is not Glass _) return;
        //     
        //     //e.Pointer.Capture(null);
        //     _isPointerPressed = false;
        // };
        //
        // PointerMoved += (sender, e) =>
        // {
        //     if (!_isPointerPressed) return;
        //     
        //     var mousePosWindow = e.GetPosition(this);
        //     var glassPanelPosition = GlassPanel.Bounds.Position;
        //     
        //     var offsetX = mousePosWindow.X - glassPanelPosition.X;
        //     var offsetY = mousePosWindow.Y - glassPanelPosition.Y;
        //     
        //     GlassPanel.RenderTransform = new TranslateTransform(
        //         offsetX,
        //         offsetY);
        // };
    }
}