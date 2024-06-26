using CommunityToolkit.HighPerformance.Buffers;
using TotkCommon;

namespace TotkZstdTool.Helpers;

public static class ZstdHelper
{
    public static int GetDictioanryId(this string path, bool useDictionaries)
    {
        return useDictionaries switch
        {
            false => -1,
            true => path.EndsWith(".rsizetable") || path.EndsWith("ZsDic.pack") ? -1 :
                    path.EndsWith(".bcett.byml") ? 2 :
                    path.EndsWith(".pack") ? 3 : 1
        };
    }

    public static void Decompress(string file, string output)
    {
        using SpanOwner<byte> decompressed = Decompress(file);
        using FileStream fs = File.Create(output);
        fs.Write(decompressed.Span);
    }

    public static SpanOwner<byte> Decompress(string file)
    {
        using FileStream fs = File.OpenRead(file);
        int size = Convert.ToInt32(fs.Length);
        using SpanOwner<byte> buffer = SpanOwner<byte>.Allocate(size);
        fs.Read(buffer.Span);

        size = Zstd.GetDecompressedSize(buffer.Span);
        SpanOwner<byte> decompressed = SpanOwner<byte>.Allocate(size);
        Totk.Zstd.Decompress(buffer.Span, decompressed.Span);
        return decompressed;
    }

    public static void Compress(string file, string output, bool useDictionaries)
    {
        using SpanOwner<byte> compressed = Compress(file, useDictionaries);
        using FileStream fs = File.Create(output);
        fs.Write(compressed.Span);
    }

    public static SpanOwner<byte> Compress(string file, bool useDictionaries)
    {
        using FileStream fs = File.OpenRead(file);
        int size = Convert.ToInt32(fs.Length);
        using SpanOwner<byte> buffer = SpanOwner<byte>.Allocate(size);
        fs.Read(buffer.Span);

        size = Zstd.GetDecompressedSize(buffer.Span);
        SpanOwner<byte> decompressed = SpanOwner<byte>.Allocate(size);
        Totk.Zstd.Decompress(buffer.Span, decompressed.Span);
        return decompressed;
    }

    public static void DecompressFolder(string path, string output, bool recursive, Action<int>? setCount = null, Action<int>? updateCount = null)
    {
        string[] files = Directory.GetFiles(path, "*.zs", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        setCount?.Invoke(files.Length);

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            using SpanOwner<byte> data = Decompress(file);

            string outputFile = Path.Combine(output, Path.GetRelativePath(path, file.Remove(file.Length - 3, 3)));
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
            using FileStream fs = File.Create(outputFile);
            fs.Write(data.Span);

            updateCount?.Invoke(i + 1);
        }
    }

    public static void CompressFolder(string path, string output, bool recursive, Action<int>? setCount = null, Action<int>? updateCount = null, bool useDictionaries = true)
    {
        string[] files = Directory.GetFiles(path, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        setCount?.Invoke(files.Length);

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            using SpanOwner<byte> data = Compress(file, useDictionaries);

            string outputFile = Path.Combine(output, Path.GetRelativePath(path, $"{file}.zs"));
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
            using FileStream fs = File.Create(outputFile);
            fs.Write(data.Span);

            updateCount?.Invoke(i + 1);
        }
    }
}