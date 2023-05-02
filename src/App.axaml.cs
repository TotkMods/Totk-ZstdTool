using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Totk.ZStdTool.ViewModels;
using Totk.ZStdTool.Views;

namespace Totk.ZStdTool;

public partial class App : Application
{
    public static TotkConfig Config { get; } = TotkConfig.Load();
    public static TopLevel? VisualRoot { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new ShellView {
                DataContext = ShellViewModel.Shared,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}