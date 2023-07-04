using Totk.ZStdTool.Helpers;

namespace Totk.ZStdTool;

public static class CommandProcessor
{
    // c|compress <file-path> [-o|--output] [-h|--help]
    // d|decompress <file-path> [-o|--output] [-h|--help]

    public static void Process(List<string> args)
    {
        string[] files = args.Where(File.Exists).ToArray();
        if (files.Length == args.Count) {
            foreach (string file in files) {
                ProcessShellInput(file);
            }

            return;
        }
        
        if (args[0].AsFlag() == 'h') {
            Console.WriteLine("""
                Compress a file:
                    c, compress <file-path> [-o|--output] [-d|--dictionaries] [-h|--help]

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

    public static void ProcessShellInput(string input)
    {
        if (input.EndsWith(".zs")) {
            Decompress(input, new());
        }
        else {
            Compress(input, new());
        }
    }

    public static void Compress(string input, Dictionary<char, string> flags)
    {
        flags.TryGetValue('o', out string? output);
        output ??= input + ".zs";

        bool useDictionaries = true;
        if (flags.TryGetValue('d', out string? useDictionariesArg)) {
            useDictionaries = useDictionariesArg.ToLower() is not "f" or "false" or "n" or "no";
        }

        if (Path.GetDirectoryName(output) is string directory && !string.IsNullOrEmpty(directory)) {
            Directory.CreateDirectory(directory);
        }

        using FileStream fs = File.Create(output);
        fs.Write(ZStdHelper.Compress(input, useDictionaries));
    }

    public static void Decompress(string input, Dictionary<char, string> flags)
    {
        flags.TryGetValue('o', out string? output);
        output ??= Path.ChangeExtension(input, string.Empty);

        if (Path.GetDirectoryName(output) is string directory && !string.IsNullOrEmpty(directory)) {
            Directory.CreateDirectory(directory);
        }

        using FileStream fs = File.Create(output);
        fs.Write(ZStdHelper.Decompress(input));
    }
}
