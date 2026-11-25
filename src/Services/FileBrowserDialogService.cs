
namespace OpenCreativeSuite.Components.Files
{
    /// <summary>
    /// Opens modals through a single host mounted near the root of the app.
    /// This avoids nested fixed-position boxes being trapped by scroll or
    /// transformed ancestors in the Donkeh layout engine.
    /// </summary>
    public static class FileBrowserDialogService
    {
        public static event Action<FileBrowserDialogRequest>? Requested;

        public static bool HasListeners => Requested != null;

        public static void Open(FileBrowserDialogRequest request) => Requested?.Invoke(request);
    }
}
