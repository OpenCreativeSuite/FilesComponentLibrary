
namespace OpenCreativeSuite.Files
{
    public sealed class RecentItemsService
    {
        public static RecentItemsService Default { get; } = new RecentItemsService();

        public int MaxItems { get; set; } = 20;

        private readonly List<FileSystemEntry> _files = new();
        private readonly List<FileSystemEntry> _directories = new();

        public event Action? Changed;

        public IReadOnlyList<FileSystemEntry> RecentFiles => _files.AsReadOnly();
        public IReadOnlyList<FileSystemEntry> RecentDirectories => _directories.AsReadOnly();

        public void AddFile(FileSystemEntry entry)
        {
            _files.RemoveAll(e => string.Equals(e.FullPath, entry.FullPath, StringComparison.OrdinalIgnoreCase));
            _files.Insert(0, entry);
            Trim(_files);
            Changed?.Invoke();
        }

        public void AddDirectory(FileSystemEntry entry)
        {
            _directories.RemoveAll(e => string.Equals(e.FullPath, entry.FullPath, StringComparison.OrdinalIgnoreCase));
            _directories.Insert(0, entry);
            Trim(_directories);
            Changed?.Invoke();
        }

        public void Remove(FileSystemEntry entry)
        {
            _files.RemoveAll(e => string.Equals(e.FullPath, entry.FullPath, StringComparison.OrdinalIgnoreCase));
            _directories.RemoveAll(e => string.Equals(e.FullPath, entry.FullPath, StringComparison.OrdinalIgnoreCase));
            Changed?.Invoke();
        }

        public void ClearFiles()
        {
            _files.Clear();
            Changed?.Invoke();
        }

        public void ClearDirectories()
        {
            _directories.Clear();
            Changed?.Invoke();
        }

        public void ClearAll()
        {
            _files.Clear();
            _directories.Clear();
            Changed?.Invoke();
        }

        private void Trim(List<FileSystemEntry> list)
        {
            while (list.Count > MaxItems) list.RemoveAt(list.Count - 1);
        }
    }
}