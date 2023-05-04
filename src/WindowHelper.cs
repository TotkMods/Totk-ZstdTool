using System.Runtime.InteropServices;

namespace Totk.ZStdTool;

public enum WindowMode : int { Hidden = 0, Visible = 5 }

public static partial class WindowHelper
{
    [LibraryImport("kernel32.dll")]
    private static partial IntPtr GetConsoleWindow();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private static readonly IntPtr Handle = GetConsoleWindow();
    public static void SetWindowMode(WindowMode mode)
    {
        ShowWindow(Handle, (int)mode);
    }
}
