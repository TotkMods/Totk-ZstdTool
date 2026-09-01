using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.Win32;

namespace TotkZstdTool.Helpers;

[SupportedOSPlatform("windows")]
public static class ContextMenuHelper
{
    private const string CompressClass = "*";
    private const string DecompressClass = @"SystemFileAssociations\.zs";

    private const string CompressName = "compress";
    private const string DecompressName = "decompress";

    public static bool IsInstalled()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        using RegistryKey? compress = Registry.ClassesRoot.OpenSubKey($@"{CompressClass}\shell\totk\shell\{CompressName}");
        using RegistryKey? decompress = Registry.ClassesRoot.OpenSubKey($@"{DecompressClass}\shell\totk\shell\{DecompressName}");
        return compress is not null && decompress is not null;
    }

    public static void Install()
    {
        RunElevated(install: true);
    }

    public static void Uninstall()
    {
        RunElevated(install: false);
    }

    private static void RunElevated(bool install)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        if (!IsElevated())
        {
            RelaunchElevated(install ? "install-context-menu" : "uninstall-context-menu");
            return;
        }

        if (install)
        {
            InstallCore();
        }
        else
        {
            UninstallCore();
        }
    }

    private static bool IsElevated()
    {
        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static void RelaunchElevated(string args)
    {
        string exe = Environment.ProcessPath
            ?? throw new InvalidOperationException("Could not resolve the path to the running executable.");

        ProcessStartInfo psi = new(exe, args)
        {
            UseShellExecute = true,
            Verb = "runas",
        };

        try
        {
            using Process? process = Process.Start(psi);
            process?.WaitForExit();
        }
        catch (Win32Exception)
        {
            // The user declined the UAC prompt.
        }
    }

    private static void InstallCore()
    {
        string exe = Environment.ProcessPath!;

        using (RegistryKey shell = GetTotkShell(CompressClass))
        using (RegistryKey key = shell.CreateSubKey(CompressName))
        {
            key.SetValue(string.Empty, "Compress with TotK Zstd Tool");
            key.SetValue("Icon", $"\"{exe}\",0");
            key.SetValue("AppliesTo", "NOT System.FileExtension:=.zs");

            using RegistryKey command = key.CreateSubKey("command");
            command.SetValue(string.Empty, $"\"{exe}\" compress \"%1\"");
        }

        using (RegistryKey shell = GetTotkShell(DecompressClass))
        using (RegistryKey key = shell.CreateSubKey(DecompressName))
        {
            key.SetValue(string.Empty, "Decompress with TotK Zstd Tool");
            key.SetValue("Icon", $"\"{exe}\",0");

            using RegistryKey command = key.CreateSubKey("command");
            command.SetValue(string.Empty, $"\"{exe}\" decompress \"%1\"");
        }
    }

    private static void UninstallCore()
    {
        DeleteEntry(CompressClass, CompressName);
        DeleteEntry(DecompressClass, DecompressName);
    }

    private static RegistryKey GetTotkShell(string cls)
    {
        using RegistryKey root = Registry.ClassesRoot.CreateSubKey($@"{cls}\shell\totk");
        root.SetValue("MUIVerb", "TotK", RegistryValueKind.String);
        root.SetValue("SubCommands", string.Empty, RegistryValueKind.String);

        return root.CreateSubKey("shell");
    }

    private static void DeleteEntry(string cls, string name)
    {
        using RegistryKey? shell = Registry.ClassesRoot.OpenSubKey($@"{cls}\shell\totk\shell", writable: true);
        shell?.DeleteSubKeyTree(name, throwOnMissingSubKey: false);
    }
}
