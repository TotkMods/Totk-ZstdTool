using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Totk.ZStdTool.Helpers;

namespace Totk.ZStdTool.ViewModels;

public class ShellViewModel : ReactiveObject
{
    public static ShellViewModel Shared { get; } = new();

    private string _filePath = string.Empty;
    public string FilePath {
        get => _filePath;
        set {
            this.RaiseAndSetIfChanged(ref _filePath, value);
            this.RaiseAndSetIfChanged(ref _canDecompress, File.Exists(value) && Path.GetExtension(value) == ".zs", nameof(CanDecompress));
            this.RaiseAndSetIfChanged(ref _canCompress, File.Exists(value) && Path.GetExtension(value) != ".zs", nameof(CanCompress));
        }
    }

    private bool _canDecompress = false;
    public bool CanDecompress => _canDecompress;
    private bool _canCompress = false;
    public bool CanCompress => _canCompress;

    private bool _decompressRecursive = true;
    public bool DecompressRecursive {
        get => _decompressRecursive;
        set => this.RaiseAndSetIfChanged(ref _decompressRecursive, value);
    }

    private string _folderPath = string.Empty;
    public string FolderPath {
        get => _folderPath;
        set {
            this.RaiseAndSetIfChanged(ref _folderPath, value);
            this.RaiseAndSetIfChanged(ref _canDecompressFolder, Directory.Exists(value), nameof(CanDecompressFolder));
            this.RaiseAndSetIfChanged(ref _canCompressFolder, Directory.Exists(value), nameof(CanCompressFolder));
        }
    }

    private bool _canDecompressFolder;
    public bool CanDecompressFolder => _canDecompressFolder;
    private bool _canCompressFolder;
    public bool CanCompressFolder => _canCompressFolder;

    public async Task Browse(object param)
    {
        var mode = param as string;
        if (mode == "File") {
            BrowserDialog dialog = new(BrowserMode.OpenFile, "Open zStd File", "zStd Files:*.zs", instanceBrowserKey: "load");
            if (await dialog.ShowDialog() is string path) {
                FilePath = path;
            }
        }
        else if (mode == "Folder") {
            BrowserDialog dialog = new(BrowserMode.OpenFolder, "Open Folder", instanceBrowserKey: "load-fld");
            if (await dialog.ShowDialog() is string path) {
                FolderPath = path;
            }
        }

        throw new NotImplementedException($"The browse mode '{mode}' is not implemented");
    }

    public async Task Decompress()
    {
        try {
            string outputFile = Path.GetFileNameWithoutExtension(FilePath);
            BrowserDialog dialog = new(BrowserMode.SaveFile, "Save Decompressed File", $"Raw File:*{Path.GetExtension(outputFile)}|Any File:*.*", outputFile, "save");
            if (await dialog.ShowDialog() is string path) {
                StartLoading(1);

                using FileStream fs = File.Create(path);
                fs.Write(ZStdHelper.Decompress(FilePath));

                UpdateCount(1);

                ContentDialog dlg = new() {
                    Content = $"File Decompressed to '{path}'",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Close",
                    Title = "Notice"
                };

                await dlg.ShowAsync();
                StopLoading();
            }
        }
        catch (Exception ex) {
            ContentDialog dlg = new() {
                Content = new TextBox {
                    MaxHeight = 250,
                    Text = ex.ToString(),
                    IsReadOnly = true,
                },
                PrimaryButtonText = "OK",
                Title = "Unhandled Exception"
            };

            await dlg.ShowAsync();
        }
    }
    
    public async Task Compress()
    {
        try
        {
            string outputFile = $"{Path.GetFileName(FilePath)}.zs";
            BrowserDialog dialog = new(BrowserMode.SaveFile, "Save Compressed File", $"Zstd Compressed File:*.zs|Any File:*.*", outputFile, "save");
            if (await dialog.ShowDialog() is string path)
            {
                using FileStream fs = File.Create(path);
                fs.Write(ZStdHelper.Compress(FilePath));

                ContentDialog dlg = new()
                {
                    Content = $"File compressed to '{path}'",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Close",
                    Title = "Notice"
                };

                await dlg.ShowAsync();
            }
        }
        catch (Exception ex)
        {
            ContentDialog dlg = new()
            {
                Content = new TextBox
                {
                    MaxHeight = 250,
                    Text = ex.ToString(),
                    IsReadOnly = true,
                },
                PrimaryButtonText = "OK",
                Title = "Unhandled Exception"
            };

            await dlg.ShowAsync();
        }
    }

    public async Task DecompressFolder()
    {
        try {
            string outputFile = Path.GetFileNameWithoutExtension(FilePath);
            BrowserDialog dialog = new(BrowserMode.OpenFolder, "Output Folder", "save-fld");
            if (await dialog.ShowDialog() is string path && Directory.Exists(path)) {
                StartLoading();
                await Task.Run(() => ZStdHelper.DecompressFolder(FolderPath, path, DecompressRecursive, SetCount, UpdateCount));

                ContentDialog dlg = new() {
                    Content = $"Folder Decompressed to '{path}'",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Close",
                    Title = "Notice"
                };

                await dlg.ShowAsync();
                StopLoading();
            }
        }
        catch (Exception ex) {
            ContentDialog dlg = new() {
                Content = new TextBox {
                    MaxHeight = 250,
                    Text = ex.ToString(),
                    IsReadOnly = true,
                },
                PrimaryButtonText = "OK",
                Title = "Unhandled Exception"
            };

            await dlg.ShowAsync();
        }
    }

    public async Task CompressFolder()
    {
        try {
            string outputFile = Path.GetFileNameWithoutExtension(FilePath);
            BrowserDialog dialog = new(BrowserMode.OpenFolder, "Output Folder", "save-fld");
            if (await dialog.ShowDialog() is string path && Directory.Exists(path)) {
                await Task.Run(() => ZStdHelper.CompressFolder(FolderPath, path, DecompressRecursive));

                ContentDialog dlg = new() {
                    Content = $"Folder compressed to '{path}'",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Close",
                    Title = "Notice"
                };

                await dlg.ShowAsync();
            }
        }
        catch (Exception ex) {
            ContentDialog dlg = new() {
                Content = new TextBox {
                    MaxHeight = 250,
                    Text = ex.ToString(),
                    IsReadOnly = true,
                },
                PrimaryButtonText = "OK",
                Title = "Unhandled Exception"
            };

            await dlg.ShowAsync();
        }
    }

    public static async Task ShowSettings()
    {
        ContentDialog dlg = new() {
            Content = new ScrollViewer {
                MaxHeight = 250,
                Content = new StackPanel {
                    Margin = new(0, 0, 15, 0),
                    Spacing = 10,
                    Children = {
                        new TextBox {
                            Text = App.Config.GamePath,
                            Watermark = "Game Path",
                            UseFloatingWatermark = true,
                        },
                    }
                }
            },
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Cancel",
            Title = "Settings"
        };

        if (await dlg.ShowAsync() == ContentDialogResult.Primary) {
            var stack = (dlg.Content as ScrollViewer)!.Content as StackPanel;
            App.Config.GamePath = (stack!.Children[0] as TextBox)!.Text!;
            App.Config.Save();
        }
    }

    //
    // Loading stuff

    private readonly DispatcherTimer _timer = new() {
        Interval = new(0, 0, 0, 0, 500),
    };

    public ShellViewModel()
    {
        _timer.Tick += (s, e) => {
            LoadingDots += " .";
            LoadingDots = LoadingDots.Replace(" . . . . .", " .");
        };
    }

    private bool _isLoading = false;
    public bool IsLoading {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private string _loadingDots = " . . . .";
    public string LoadingDots {
        get => _loadingDots;
        set => this.RaiseAndSetIfChanged(ref _loadingDots, value);
    }

    private string _processCount = "-/-";
    public string ProcessCount {
        get => _processCount;
        set => this.RaiseAndSetIfChanged(ref _processCount, value);
    }

    public void StartLoading(int? initCount = null)
    {
        _timer.Start();
        IsLoading = true;

        if (initCount is int num) {
            SetCount(num);
        }
    }

    public void StopLoading()
    {
        _timer.Stop();
        IsLoading = false;
        ProcessCount = "-/-";
    }

    public void SetCount(int num)
    {
        _processCount = _processCount.Split('/')[0];
        ProcessCount = $"{_processCount}/{num}";
    }

    public void UpdateCount(int num)
    {
        _processCount = _processCount.Split('/')[1];
        ProcessCount = $"{num}/{_processCount}";
    }
}
