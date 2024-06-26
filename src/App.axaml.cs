using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using TotkZstdTool.ViewModels;
using TotkZstdTool.Views;

namespace TotkZstdTool;

public partial class App : Application
{
    public static TopLevel? VisualRoot { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new ShellView
            {
                DataContext = ShellViewModel.Shared,
            };

            VisualRoot = desktop.MainWindow.GetVisualRoot() as TopLevel;
        }

        base.OnFrameworkInitializationCompleted();
    }
}