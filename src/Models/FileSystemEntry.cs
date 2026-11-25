namespace OpenCreativeSuite.Components.Files
{
    public sealed record FileSystemEntry(
        string Name,
        string FullPath,
        bool IsDirectory,
        long Size = 0,
        DateTime Modified = default,
        string Extension = "",
        string? MimeType = null);
}