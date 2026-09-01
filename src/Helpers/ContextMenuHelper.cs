using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.Win32;

namespace TotkZstdTool.Helpers;

[SupportedOSPlatform("windows")]
public static class ContextMenuHelper
{
    // Matches the class/name TotkRegistryToolkit registers its own "ZSTD" feature under, so installing
    // either tool's context menu entry resolves to the same key instead of creating a duplicate.
    // https://github.com/ArchLeaders/TotkRegistryToolkit
    private const string ShellClass = "*";
    private const string ShellName = "zstd";

    public static bool IsInstalled()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        using RegistryKey? key = Registry.ClassesRoot.OpenSubKey($@"{ShellClass}\shell\totk\shell\{ShellName}");
        return key is not null;
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

        using RegistryKey shell = GetTotkShell(ShellClass);
        using RegistryKey key = shell.CreateSubKey(ShellName);
        key.SetValue(string.Empty, "ZSTD De/Compress");
        key.SetValue("Icon", $"\"{exe}\",0");

        // No verb: passing a bare path routes through CommandProcessor.ProcessShellInput, which
        // picks compress or decompress by extension, same as TotkRegistryToolkit's ZsFeature.
        using RegistryKey command = key.CreateSubKey("command");
        command.SetValue(string.Empty, $"\"{exe}\" \"%1\"");
    }

    private static void UninstallCore()
    {
        DeleteEntry(ShellClass, ShellName);
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
