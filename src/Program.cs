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

    private static void ProcessArgs(List<string> args)
    {
        DllManager.LoadCead();

        string? infile = null;
        string? outFile = null;

        while (args.Any()) {
            string entry = args.First();
            args.RemoveAt(0);

            bool isFlag = entry.StartsWith('-') || entry.StartsWith('/');
            if (isFlag) {
                string f = entry[1..].ToLower();
                switch (f) {
                    case "x":
                        string outDecPath = outFile ?? Path.Combine(Path.GetDirectoryName(entry)!, Path.GetFileNameWithoutExtension(entry));
                        Decompress(infile, outDecPath);
                        continue;
                    case "c":
                        string outCompPath = outFile ?? entry + ".zs";
                        Compress(infile, outCompPath);
                        continue;
                    case "o":
                        outFile = args.First();
                        args.RemoveAt(0);
                        continue;
                    case "i":
                        infile = args.First();
                        args.RemoveAt(0);
                        continue;
                }
            }

            if (File.Exists(entry)) {
                infile = entry;
            }
        }
    }

    public static void Compress(string inputPath, string outputPath)
    {
        if (inputPath == null || outputPath == null) {
            return;
        }
        Span<byte> data = ZStdHelper.Compress(inputPath);
        File.WriteAllBytes(outputPath, data.ToArray());
    }

    public static void Decompress(string inputPath, string outputPath)
    {
        if (inputPath == null || outputPath == null) {
            return;
        }
        Span<byte> data = ZStdHelper.Decompress(inputPath);
        File.WriteAllBytes(outputPath, data.ToArray());
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
