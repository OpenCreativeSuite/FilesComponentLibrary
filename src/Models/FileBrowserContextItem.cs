
namespace OpenCreativeSuite.Components.Files
{
    public sealed class FileBrowserContextRequest
    {
        public IReadOnlyList<FileSystemEntry> Selection { get; set; } = Array.Empty<FileSystemEntry>();
        public FileSystemEntry? Target { get; set; }
        public string CurrentPath { get; set; } = "";
        public object? Browser { get; set; }
    }

    public sealed class FileBrowserContextItem
    {
        public string Label { get; set; } = "";
        public string? Icon { get; set; }
        public bool IsSeparator { get; set; }
        public string? Shortcut { get; set; }
        public IReadOnlyList<FileBrowserContextItem>? Submenu { get; set; }

        public string[]? Extensions { get; set; }
        public string[]? MimeTypes { get; set; }
        public bool? ForDirectory { get; set; }
        public string[]? ExactNames { get; set; }
        public Func<FileSystemEntry, bool>? Predicate { get; set; }
        public int? MinSelection { get; set; }
        public int? MaxSelection { get; set; }
        public int Order { get; set; }

        public Func<FileBrowserContextRequest, bool>? IsVisible { get; set; }
        public Func<FileBrowserContextRequest, bool>? IsEnabled { get; set; }
        public Action<FileBrowserContextRequest>? OnClick { get; set; }

        public bool Applies(FileBrowserContextRequest request)
        {
            var selection = request.Selection ?? Array.Empty<FileSystemEntry>();
            var entries = selection.Count > 0
                ? selection.ToList()
                : (request.Target != null ? new List<FileSystemEntry> { request.Target } : new List<FileSystemEntry>());

            if (MinSelection.HasValue && selection.Count < MinSelection.Value) return false;
            if (MaxSelection.HasValue && selection.Count > MaxSelection.Value) return false;

            if (entries.Count > 0)
            {
                foreach (var entry in entries)
                {
                    if (ForDirectory.HasValue && entry.IsDirectory != ForDirectory.Value) return false;
                    if (Extensions != null && Extensions.Length > 0)
                    {
                        var ext = entry.Extension;
                        if (string.IsNullOrEmpty(ext)) ext = System.IO.Path.GetExtension(entry.Name);
                        if (!Extensions.Any(e => string.Equals(e, ext, StringComparison.OrdinalIgnoreCase))) return false;
                    }
                    if (MimeTypes != null && MimeTypes.Length > 0 && entry.MimeType != null)
                    {
                        if (!MimeTypes.Any(m => string.Equals(m, entry.MimeType, StringComparison.OrdinalIgnoreCase))) return false;
                    }
                    if (ExactNames != null && ExactNames.Length > 0)
                    {
                        if (!ExactNames.Any(n => string.Equals(n, entry.Name, StringComparison.OrdinalIgnoreCase))) return false;
                    }
                    if (Predicate != null && !Predicate(entry)) return false;
                }
            }

            if (IsVisible != null && !IsVisible(request)) return false;
            return true;
        }

        public bool Enabled(FileBrowserContextRequest request)
        {
            if (IsEnabled != null) return IsEnabled(request);
            return true;
        }
    }

    public sealed class FileBrowserContextMenuState
    {
        public double X { get; set; }
        public double Y { get; set; }
        public IReadOnlyList<FileBrowserContextItem> Items { get; set; } = Array.Empty<FileBrowserContextItem>();
        public FileBrowserContextRequest Request { get; set; } = new();
    }
}