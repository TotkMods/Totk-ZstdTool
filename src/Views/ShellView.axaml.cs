using Avalonia.Controls;
using Avalonia.Input;

namespace Totk.ZStdTool.Views;
public partial class ShellView : Window
{
    public ShellView()
    {
        InitializeComponent();
        FileNameEntry.AddHandler(DragDrop.DropEvent, DragFile);
        FileNameEntry.AddHandler(DragDrop.DropEvent, DragFile);
    }

    public void DragFile(object? sender, DragEventArgs e)
    {
        (sender as TextBox)!.Text = e.Data?.GetFiles()?.FirstOrDefault()?.Path.LocalPath ?? string.Empty;
    }
}
