using Avalonia;
using Avalonia.ReactiveUI;

namespace TotkZstdTool;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            try
            {
                CommandProcessor.Process(args.ToList());
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TotkZstdTool", "error.log");

                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, $"{DateTime.Now}\nArgs: {string.Join(' ', args)}\n\n{ex}");

                Console.Error.WriteLine(ex);
                Environment.Exit(1);
            }
        }
        else
        {
            if (OperatingSystem.IsWindows())
            {
                WindowHelper.SetWindowMode(WindowMode.Hidden);
            }

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}
