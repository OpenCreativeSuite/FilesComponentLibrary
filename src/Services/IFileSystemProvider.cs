
namespace OpenCreativeSuite.Components.Files
{
    public interface IFileSystemProvider
    {
        Task<IReadOnlyList<FileSystemEntry>> EnumerateAsync(string path, CancellationToken cancellationToken = default);
        Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);
        Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);
        Task<FileSystemEntry?> GetEntryAsync(string path, CancellationToken cancellationToken = default);
        string GetParentPath(string path);
        bool IsRoot(string path);
        IReadOnlyList<DirectoryShortcut> GetCommonDirectories();
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
        Task RenameAsync(string path, string newName, CancellationToken cancellationToken = default);
        Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
    }

    public sealed class LocalFileSystemProvider : IFileSystemProvider
    {
        public Task<IReadOnlyList<FileSystemEntry>> EnumerateAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var result = new List<FileSystemEntry>();
                if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Directory not found: {path}");
                var dirInfo = new DirectoryInfo(path);
                foreach (var d in dirInfo.EnumerateDirectories())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    result.Add(new FileSystemEntry(d.Name, d.FullName, true, 0, d.LastWriteTime));
                }
                foreach (var f in dirInfo.EnumerateFiles())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    result.Add(new FileSystemEntry(f.Name, f.FullName, false, f.Length, f.LastWriteTime, f.Extension, GuessMime(f.Extension)));
                }
                return (IReadOnlyList<FileSystemEntry>)result;
            }, cancellationToken);
        }

        public Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
            => Task.FromResult(File.Exists(path));

        public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
            => Task.FromResult(Directory.Exists(path));

        public Task<FileSystemEntry?> GetEntryAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                if (Directory.Exists(path))
                {
                    var d = new DirectoryInfo(path);
                    return new FileSystemEntry(d.Name, d.FullName, true, 0, d.LastWriteTime);
                }
                if (File.Exists(path))
                {
                    var f = new FileInfo(path);
                    return new FileSystemEntry(f.Name, f.FullName, false, f.Length, f.LastWriteTime, f.Extension, GuessMime(f.Extension));
                }
                return null;
            }, cancellationToken);
        }

        public string GetParentPath(string path)
        {
            try
            {
                var parent = Directory.GetParent(path);
                return parent?.FullName ?? path;
            }
            catch
            {
                return path;
            }
        }

        public bool IsRoot(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return true;
            try
            {
                var root = Path.GetPathRoot(path);
                return string.Equals(Path.GetFullPath(path), root, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                if (File.Exists(path)) File.Delete(path);
                else if (Directory.Exists(path)) Directory.Delete(path, true);
            }, cancellationToken);
        }

        public Task RenameAsync(string path, string newName, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var parent = Path.GetDirectoryName(path) ?? "";
                var dest = Path.Combine(parent, newName);
                if (File.Exists(path)) File.Move(path, dest);
                else if (Directory.Exists(path)) Directory.Move(path, dest);
            }, cancellationToken);
        }

        public Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destinationPath, true);
                }
                else if (Directory.Exists(sourcePath))
                {
                    CopyDirectory(sourcePath, destinationPath);
                }
            }, cancellationToken);
        }

        public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Directory.CreateDirectory(path), cancellationToken);
        }

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.EnumerateFiles(source))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.EnumerateDirectories(source))
            {
                CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
            }
        }

        public IReadOnlyList<DirectoryShortcut> GetCommonDirectories()
        {
            var list = new List<DirectoryShortcut>();
            TryAdd(list, "Home", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            TryAdd(list, "Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            TryAdd(list, "Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            TryAdd(list, "Downloads", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
            TryAdd(list, "Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            TryAdd(list, "Music", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            TryAdd(list, "Videos", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            // roots
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.IsReady)
                        list.Add(new DirectoryShortcut(drive.Name, drive.RootDirectory.FullName));
                }
                catch { }
            }
            return list;
        }

        private static void TryAdd(List<DirectoryShortcut> list, string label, string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                list.Add(new DirectoryShortcut(label, path));
        }

        private static string GuessMime(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                ".txt" => "text/plain",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".html" or ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "text/javascript",
                _ => "application/octet-stream"
            };
        }
    }
}