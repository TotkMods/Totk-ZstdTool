using Avalonia.Platform.Storage;

namespace Totk.ZStdTool.Helpers;

public enum BrowserMode { OpenFile, OpenFolder, SaveFile }

public class BrowserDialog
{
    private static Dictionary<string, BrowseHistory> Stashed { get; set; } = new();
    private record BrowseHistory()
    {
        public IStorageFolder? OpenDirectory { get; set; } = null;
        public IStorageFolder? SaveDirectory { get; set; } = null;
    }

    public static IStorageFolder? LastOpenDirectory { get; set; }
    public static IStorageFolder? LastSaveDirectory { get; set; }

    private readonly BrowserMode Mode;
    private string? Title;
    private readonly string? Filter;
    private readonly string? InstanceBrowserKey;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="title"></param>
    /// <param name="filter">Semicolon delimited list of file filters. (Syntax: <c>Yaml Files<see cref="char">:</see>*.yml<see cref="char">;</see>*.yaml<see cref="char">|</see>All Files<see cref="char">:</see>*.*</c>)</param>
    /// <param name="instanceBrowserKey">Saves the last open/save directory as an instance mapped to the specified key</param>
    public BrowserDialog(BrowserMode mode, string? title = null, string? filter = null, string? instanceBrowserKey = null)
    {
        Mode = mode;
        Title = title;
        Filter = filter;
        InstanceBrowserKey = instanceBrowserKey;

        if (instanceBrowserKey != null) {
            if (!Stashed.ContainsKey(instanceBrowserKey)) {
                Stashed.Add(instanceBrowserKey, new());
            }
        }
    }

    /// <inheritdoc cref="ShowDialog(bool)"/>
    public async Task<string?> ShowDialog()
    {
        return (await ShowDialog(false))?.First();
    }

    /// <summary>
    /// Opens a new <see cref="IStorageProvider"/> dialog and returns the selected files/folders.
    /// </summary>
    /// <param name="allowMultiple"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>?> ShowDialog(bool allowMultiple)
    {
        Title ??= Mode.ToString().Replace("F", " F");

        IStorageProvider StorageProvider = App.VisualRoot!.StorageProvider;

        object? result = Mode switch {
            BrowserMode.OpenFolder => await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() {
                Title = Title,
                SuggestedStartLocation = GetLastDirectory(),
                AllowMultiple = allowMultiple
            }),
            BrowserMode.OpenFile => await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() {
                Title = Title,
                SuggestedStartLocation = GetLastDirectory(),
                AllowMultiple = allowMultiple,
                FileTypeFilter = LoadFileBrowserFilter(Filter)
            }),
            BrowserMode.SaveFile => await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions() {
                Title = Title,
                SuggestedStartLocation = GetLastDirectory(),
                FileTypeChoices = LoadFileBrowserFilter(Filter)
            }),
            _ => throw new NotImplementedException()
        };

        if (result is IReadOnlyList<IStorageFolder> folders && folders.Count > 0) {
            SetLastDirectory(folders[folders.Count - 1]);
            return folders.Select(folder => folder.Path.LocalPath);
        }
        else if (result is IReadOnlyList<IStorageFile> files && files.Count > 0) {
            SetLastDirectory(await files[files.Count - 1].GetParentAsync());
            return files.Select(file => file.Path.LocalPath);
        }
        else if (result is IStorageFile file) {
            SetLastDirectory(await file.GetParentAsync());
            return new string[1] {
                file.Path.LocalPath
            };
        }
        else {
            return null;
        }
    }

    internal void SetLastDirectory(IStorageFolder? folder)
    {
        if (Mode == BrowserMode.SaveFile) {
            LastSaveDirectory = folder;
            if (InstanceBrowserKey != null) {
                Stashed[InstanceBrowserKey].SaveDirectory = folder;
            }
        }
        else {
            LastOpenDirectory = folder;
            if (InstanceBrowserKey != null) {
                Stashed[InstanceBrowserKey].OpenDirectory = folder;
            }
        }
    }

    internal IStorageFolder? GetLastDirectory()
    {
        if (Mode == BrowserMode.SaveFile) {
            if (InstanceBrowserKey != null) {
                return Stashed[InstanceBrowserKey].SaveDirectory;
            }
            else {
                return LastSaveDirectory;
            }
        }
        else {
            if (InstanceBrowserKey != null) {
                return Stashed[InstanceBrowserKey].OpenDirectory;
            }
            else {
                return LastOpenDirectory;
            }
        }
    }

    internal static FilePickerFileType[] LoadFileBrowserFilter(string? filter = null)
    {
        if (filter != null) {
            try {
                string[] groups = filter.Split('|');
                FilePickerFileType[] types = new FilePickerFileType[groups.Length];

                for (int i = 0; i < groups.Length; i++) {
                    string[] pair = groups[i].Split(':');
                    types[i] = new(pair[0]) {
                        Patterns = pair[1].Split(';')
                    };
                }

                return types;
            }
            catch {
                throw new FormatException(
                    $"Could not parse filter arguments '{filter}'.\n" +
                    $"Example: \"Yaml Files:*.yml;*.yaml|All Files:*.*\"."
                );
            }
        }

        return Array.Empty<FilePickerFileType>();
    }
}
