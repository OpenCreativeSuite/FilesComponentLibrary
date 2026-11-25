
namespace OpenCreativeSuite.Components.Files
{
    /// <summary>Describes a file/directory dialog opened through <see cref="FileBrowserDialogService"/>.</summary>
    public sealed class FileBrowserDialogRequest
    {
        public string Title { get; set; } = "Select";
        public string InitialPath { get; set; } = "";
        public FileBrowserViewMode InitialView { get; set; } = FileBrowserViewMode.List;
        public bool AllowMultiSelect { get; set; }
        public bool AllowFileSelection { get; set; } = true;
        public bool AllowDirectorySelection { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string[] ExcludedExtensions { get; set; } = Array.Empty<string>();
        public IFileSystemProvider? FileSystemProvider { get; set; }
        public RecentItemsService? RecentItemsService { get; set; }
        public FileBrowserTheme? Theme { get; set; }
        public bool DarkMode { get; set; }
        public Action<IReadOnlyList<FileSystemEntry>>? OnConfirm { get; set; }
        public Action? OnCancel { get; set; }
    }
}
