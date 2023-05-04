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
        if (args?.Length > 0) {
            ProcessArgs(args.ToList());
        }
        else {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
    }

    private static void ProcessArgs(List<string> Args) {
        DllManager.LoadCead();

        string Infile = null;
        string OutFile = null;

        while (Args.Any()) {
            string Entry = Args.First();
            Args.RemoveAt(0);

            bool IsFlag = Entry.StartsWith('-') || Entry.StartsWith('/');
            if (IsFlag) {
                var Flag = Entry.Substring(1).ToLower();
                switch (Flag) {
                    case "x":
                        string OutDecPath = OutFile ?? Path.Combine(Path.GetDirectoryName(Entry), Path.GetFileNameWithoutExtension(Entry));
                        Decompress(Infile, OutDecPath);
                        continue;
                    case "c":
                        string OutCompPath = OutFile ?? Entry + ".zs";
                        Compress(Infile, OutCompPath);
                        continue;
                    case "o":
                        OutFile = Args.First();
                        Args.RemoveAt(0);
                        continue;
                    case "i":
                        Infile = Args.First();
                        Args.RemoveAt(0);
                        continue;
                }
            }

            if (File.Exists(Entry)) {
                Infile = Entry;
            }
        }
    }

    public static void Compress(string InputPath, string OutputPath) {
        if (InputPath == null || OutputPath == null) {
            return;
        }
        var Data = ZStdHelper.Compress(InputPath);
        File.WriteAllBytes(OutputPath, Data.ToArray());
    }

    public static void Decompress(string InputPath, string OutputPath) {
        if (InputPath == null || OutputPath == null) {
            return;
        }
        var Data = ZStdHelper.Decompress(InputPath);
        File.WriteAllBytes(OutputPath, Data.ToArray());
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
