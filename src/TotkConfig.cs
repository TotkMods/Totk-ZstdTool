using System.Text.Json;

namespace Totk.ZStdTool;

public class TotkConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Totk", "config.json");

    public required string GamePath { get; set; }

    public static TotkConfig Load()
    {
        if (!File.Exists(_path)) {
            return Create();
        }

        using FileStream fs = File.OpenRead(_path);
        return JsonSerializer.Deserialize<TotkConfig>(fs) ?? Create();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using FileStream fs = File.Create(_path);
        JsonSerializer.Serialize(fs, this);
    }

    private static TotkConfig Create()
    {
        TotkConfig config = new() {
            GamePath = string.Empty
        };

        config.Save();
        return config;
    }
}
