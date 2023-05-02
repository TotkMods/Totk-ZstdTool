using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System.Diagnostics;
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
            this.RaiseAndSetIfChanged(ref _canDecompress, File.Exists(value), nameof(CanDecompress));
        }
    }

    private bool _canDecompress = false;
    public bool CanDecompress => _canDecompress;

    public async Task Browse()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Open zStd File", "zStd Files:*.zs", instanceBrowserKey: "load");
        if (await dialog.ShowDialog() is string path) {
            FilePath = path;
        }
    }

    public async Task Decompress()
    {
        try {
            string outputFile = Path.GetFileNameWithoutExtension(FilePath);
            BrowserDialog dialog = new(BrowserMode.SaveFile, "Save Decompressed File", $"Raw File:*{Path.GetExtension(outputFile)}|Any File:*.*", outputFile, "save");
            if (await dialog.ShowDialog() is string path) {
                using FileStream fs = File.Create(path);
                fs.Write(ZStdHelper.Decompress(FilePath));

                ContentDialog dlg = new() {
                    Content = $"File Decompressed to '{path}'",
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
            var stack = dlg.Content as StackPanel;
            App.Config.GamePath = (stack!.Children[0] as TextBox)!.Text!;
            App.Config.Save();
        }
    }
}
