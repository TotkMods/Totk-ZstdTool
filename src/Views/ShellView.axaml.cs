using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FluentAvalonia.UI.Windowing;

namespace TotkZstdTool.Views;
public partial class ShellView : AppWindow
{
    public ShellView()
    {
        InitializeComponent();

        Bitmap bitmap = new(AssetLoader.Open(new Uri("avares://TotkZstdTool/Assets/icon.ico")));
        Icon = bitmap.CreateScaledBitmap(new(48, 48), BitmapInterpolationMode.HighQuality);

        FileNameEntry.AddHandler(DragDrop.DropEvent, DragDropEvent);
        FolderNameEntry.AddHandler(DragDrop.DropEvent, DragDropEvent);
    }

    public void DragDropEvent(object? sender, DragEventArgs e)
    {
        (sender as TextBox)!.Text = e.Data?.GetFiles()?.FirstOrDefault()?.Path.LocalPath ?? string.Empty;
    }
}
