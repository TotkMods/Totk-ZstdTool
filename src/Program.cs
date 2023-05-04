using Avalonia;
using Avalonia.ReactiveUI;
using Cead.Interop;
using Totk.ZStdTool.Helpers;

namespace Totk.ZStdTool;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args?.Length > 0)
        {
            CLI(args);
        }
        else
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
    }

    private static void CLI(string[] Args)
    {
        DllManager.LoadCead();

        bool CompressMode = false;

        while (Args.Any())
        {
            var Entry = Args.First();
            Args = Args.Skip(1).ToArray();

            bool IsFlag = Entry.StartsWith('-') || Entry.StartsWith('/');
            if (IsFlag)
            {
                var Flag = Entry.Substring(1).ToLower();
                switch (Flag)
                {
                    case "x":
                        CompressMode = false;
                        break;
                    case "c":
                        CompressMode = true;
                        break;
                }
                continue;
            }

            if (File.Exists(Entry))
            {
                if (CompressMode)
                {
                    string OutPath = Entry + ".zs.new";
                    var Data = ZStdHelper.Compress(Entry);
                    File.WriteAllBytes(OutPath, Data.ToArray());
                }
                else
                {
                    string OutPath = Path.Combine(Path.GetDirectoryName(Entry), Path.GetFileNameWithoutExtension(Entry));
                    var Data = ZStdHelper.Decompress(Entry);
                    File.WriteAllBytes(OutPath, Data.ToArray());
                }
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
