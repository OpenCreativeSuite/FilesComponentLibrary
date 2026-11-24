namespace OpenCreativeSuite.Files
{
    public sealed class FileBrowserOptions
    {
        public string InitialPath { get; set; } = "";
        public FileBrowserViewMode InitialView { get; set; } = FileBrowserViewMode.List;
        public bool AllowMultiSelect { get; set; } = true;
        public bool AllowFileSelection { get; set; } = true;
        public bool AllowDirectorySelection { get; set; } = true;
        public string[] AllowedExtensions { get; set; } = System.Array.Empty<string>();
        public string[] ExcludedExtensions { get; set; } = System.Array.Empty<string>();
        public int MaxRecentItems { get; set; } = 20;
    }
}