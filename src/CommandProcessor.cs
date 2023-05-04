using Totk.ZStdTool.Helpers;

namespace Totk.ZStdTool;

public static class CommandProcessor
{
    // c|compress <file-path> [-o|--output] [-h|--help]
    // d|decompress <file-path> [-o|--output] [-h|--help]

    public static void Process(List<string> args)
    {
        if (args[0].AsFlag() == 'h') {
            Console.WriteLine("""
                Compress a file:
                    c, compress <file-path> [-o|--output] [-h|--help]

                Decompress a file:
                    d, decompress <file-path> [-o|--output] [-h|--help]

                Print this help message:
                    -h, --help
                """);

            return;
        }

        Dictionary<char, string> flags = args
            .Where(x => x.StartsWith('-'))
            .Select((x, i) => (key: x.AsFlag(), value: args[args.IndexOf(x) + 1]))
            .ToDictionary(x => x.key, x => x.value);

        Action<string, Dictionary<char, string>> command = args[0][0] switch {
            'c' => Compress,
            'd' => Decompress,
            _ => throw new NotImplementedException(
                $"Invalid command '{args[0]}'. Use --help to get a list of all commands."),
        };

        command(args[1], flags);
    }

    public static char AsFlag(this string input)
    {
        return input[input.LastIndexOf('-') + 1];
    }

    public static void Compress(string input, Dictionary<char, string> flags)
    {
        flags.TryGetValue('o', out string? output);
        output ??= input + ".zs";

        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        using FileStream fs = File.Create(output);
        fs.Write(ZStdHelper.Compress(input));
    }

    public static void Decompress(string input, Dictionary<char, string> flags)
    {
        flags.TryGetValue('o', out string? output);
        output ??= Path.ChangeExtension(input, string.Empty);

        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        using FileStream fs = File.Create(output);
        fs.Write(ZStdHelper.Decompress(input));
    }
}
