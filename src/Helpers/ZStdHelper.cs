using Cead;
using ZstdSharp;

namespace Totk.ZStdTool.Helpers;

public class ZStdHelper
{
    private static readonly Decompressor _defaultDecompressor = new();
    private static readonly Decompressor _commonDecompressor = new();
    private static readonly Decompressor _mapDecompressor = new();
    private static readonly Decompressor _packDecompressor = new();
    private static readonly Compressor _defaultCompressor = new(16);
    private static readonly Compressor _commonCompressor = new(16);
    private static readonly Compressor _mapCompressor = new(16);
    private static readonly Compressor _packCompressor = new(16);

    static ZStdHelper()
    {
        Span<byte> data = _commonDecompressor.Unwrap(File.ReadAllBytes(TotkConfig.ZsDicPath));
        using Sarc sarc = Sarc.FromBinary(data);

        _commonDecompressor.LoadDictionary(sarc["zs.zsdic"]);
        _mapDecompressor.LoadDictionary(sarc["bcett.byml.zsdic"]);
        _packDecompressor.LoadDictionary(sarc["pack.zsdic"]);

        _commonCompressor.LoadDictionary(sarc["zs.zsdic"]);
        _mapCompressor.LoadDictionary(sarc["bcett.byml.zsdic"]);
        _packCompressor.LoadDictionary(sarc["pack.zsdic"]);
    }

    public static Span<byte> Decompress(string file)
    {
        Span<byte> src = File.ReadAllBytes(file);
        return
            file.EndsWith(".bcett.byml.zs") ? _mapDecompressor.Unwrap(src) :
            file.EndsWith(".pack.zs") ? _packDecompressor.Unwrap(src) :
            file.EndsWith(".rsizetable.zs") ? _defaultDecompressor.Unwrap(src) :
            _commonDecompressor.Unwrap(src);
    }
    public static Span<byte> Compress(string file, bool useDictionaries)
    {
        Span<byte> src = File.ReadAllBytes(file);
        return
            !useDictionaries ? _defaultCompressor.Wrap(src) :
            file.EndsWith(".bcett.byml") ? _mapCompressor.Wrap(src) :
            file.EndsWith(".pack") ? _packCompressor.Wrap(src) :
            file.EndsWith(".rsizetable") ? _defaultCompressor.Wrap(src) :
            _commonCompressor.Wrap(src);
    }

    public static void DecompressFolder(string path, string output, bool recursive, Action<int>? setCount = null, Action<int>? updateCount = null)
    {
        string[] files = Directory.GetFiles(path, "*.zs", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        setCount?.Invoke(files.Length);

        for (int i = 0; i < files.Length; i++) {
            var file = files[i];
            Span<byte> data = Decompress(file);

            string outputFile = Path.Combine(output, Path.GetRelativePath(path, file.Remove(file.Length - 3, 3)));
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
            using FileStream fs = File.Create(outputFile);
            fs.Write(data);

            updateCount?.Invoke(i + 1);
        }
    }

    public static void CompressFolder(string path, string output, bool recursive, Action<int>? setCount = null, Action<int>? updateCount = null, bool useDictionaries = true)
    {
        string[] files = Directory.EnumerateFiles(path, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetExtension(path) != ".zs" && Path.GetFileName(path) != "ZsDic.pack")
            .ToArray();
        setCount?.Invoke(files.Length);

        for (int i = 0; i < files.Length; i++) {
            string file = files[i];
            Span<byte> data = Compress(file, useDictionaries);

            string outputFile = Path.Combine(output, Path.GetRelativePath(path, $"{file}.zs"));
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
            using FileStream fs = File.Create(outputFile);
            fs.Write(data);

            updateCount?.Invoke(i + 1);
        }
    }
}