using Cead;
using ZstdSharp;

namespace Totk.ZStdTool.Helpers;

public class ZStdHelper
{
    private enum DecompressorMode
    {
        Common,
        Pack,
        Map
    }

    private static readonly string _zsDicPath = Path.Combine(App.Config.GamePath, "Pack", "ZsDic.pack.zs");
    private static readonly Decompressor _commonDecompressor = new();
    private static readonly Decompressor _mapDecompressor = new();
    private static readonly Decompressor _packDecompressor = new();

    static ZStdHelper()
    {
        Span<byte> data = _commonDecompressor.Unwrap(File.ReadAllBytes(_zsDicPath));
        using Sarc sarc = Sarc.FromBinary(data);

        _commonDecompressor.LoadDictionary(sarc["zs.zsdic"]);
        _mapDecompressor.LoadDictionary(sarc["bcett.byml.zsdic"]);
        _packDecompressor.LoadDictionary(sarc["pack.zsdic"]);
    }

    public static Span<byte> Decompress(string file)
    {
        Span<byte> src = File.ReadAllBytes(file);
        return
            file.EndsWith(".bcett.byml.zs") ? _mapDecompressor.Unwrap(src) :
            file.EndsWith(".pack.zs") ? _packDecompressor.Unwrap(src) :
            _commonDecompressor.Unwrap(src);
    }
}
