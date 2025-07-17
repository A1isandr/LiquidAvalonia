using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Avalonia;
using ReactiveUI.Fody.Helpers;

namespace LiquidAvalonia.Demo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    public string Text { get; set; } = 
        """
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla viverra lobortis est. Sed quis iaculis sem. Sed sit amet ipsum nisl. Sed eleifend imperdiet consectetur. Suspendisse potenti. Quisque vel ex orci. Duis mattis, orci et feugiat tristique, massa metus elementum lectus, vel sollicitudin ex dolor vitae diam. Vivamus orci ex, lobortis eget ipsum at, accumsan vestibulum leo. Sed finibus dictum eros, a commodo nunc congue vel. Aenean semper tortor ac tincidunt commodo. Suspendisse at tristique ligula. Integer in ornare augue, sit amet vulputate arcu. Nulla blandit mauris vel fermentum blandit. Quisque at aliquet turpis. Nunc ullamcorper sapien at est viverra feugiat.
        Sed condimentum lectus libero, sit amet scelerisque est faucibus tincidunt. Pellentesque scelerisque orci at orci convallis luctus. Donec et iaculis nisl. Phasellus ornare maximus augue eget tincidunt. Fusce iaculis auctor nisi id rutrum. Nulla id sem magna. Nullam non velit ut ante venenatis ultricies. Nullam dictum mi quis leo fermentum, sit amet molestie dui vehicula. Nunc consequat mi quis velit malesuada, ac malesuada magna pellentesque. Praesent tristique massa sem, a euismod dui commodo quis. Donec suscipit imperdiet odio ut aliquet. Proin bibendum lacus nec bibendum gravida. Mauris tristique suscipit aliquam. Phasellus vel lorem auctor, aliquam risus at, posuere risus. 
        """;
    
    public MainWindowViewModel()
    {
        
    }
}