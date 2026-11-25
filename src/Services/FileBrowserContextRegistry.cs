
namespace OpenCreativeSuite.Components.Files
{
    public sealed class FileBrowserContextRegistration
    {
        public string Owner { get; set; } = "";
        public IReadOnlyList<FileBrowserContextItem> Items { get; set; } = Array.Empty<FileBrowserContextItem>();
    }

    public sealed class FileBrowserContextRegistry
    {
        public static FileBrowserContextRegistry Default { get; } = new FileBrowserContextRegistry();

        private readonly List<FileBrowserContextRegistration> _registrations = new();
        private readonly object _lock = new();

        public void Register(string owner, IEnumerable<FileBrowserContextItem> items)
        {
            lock (_lock)
            {
                _registrations.RemoveAll(r => r.Owner == owner);
                _registrations.Add(new FileBrowserContextRegistration { Owner = owner, Items = items.ToList() });
            }
        }

        public void Unregister(string owner)
        {
            lock (_lock)
            {
                _registrations.RemoveAll(r => r.Owner == owner);
            }
        }

        public IReadOnlyList<FileBrowserContextItem> Resolve(FileBrowserContextRequest request, IReadOnlyList<FileBrowserContextItem> defaults)
        {
            var result = new List<FileBrowserContextItem>();
            foreach (var item in defaults)
            {
                if (item.IsSeparator || item.Applies(request))
                    result.Add(item);
            }
            List<FileBrowserContextRegistration> snapshot;
            lock (_lock) snapshot = _registrations.ToList();
            foreach (var reg in snapshot)
            {
                foreach (var item in reg.Items)
                {
                    if (item.IsSeparator || item.Applies(request))
                        result.Add(item);
                }
            }
            // remove consecutive separators
            var cleaned = new List<FileBrowserContextItem>();
            foreach (var item in result.OrderBy(i => i.Order))
            {
                if (item.IsSeparator && (cleaned.Count == 0 || cleaned[cleaned.Count - 1].IsSeparator))
                    continue;
                cleaned.Add(item);
            }
            if (cleaned.Count > 0 && cleaned[cleaned.Count - 1].IsSeparator) cleaned.RemoveAt(cleaned.Count - 1);
            return cleaned;
        }
    }
}