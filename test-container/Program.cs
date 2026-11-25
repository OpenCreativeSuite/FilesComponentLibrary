using Donkeh.Hosting;
using Donkeh.UILab;
using static Donkeh.UILab.HtmlHelper;
using static Donkeh.UILab._;
using System.Runtime.CompilerServices;
using OpenCreativeSuite.Components.Files;

[assembly: Donkeh.UILab.Stylesheet("Styles/gallery.css")]

namespace Files.TestContainer;

internal static class LibraryLoader
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        GC.KeepAlive(typeof(FileSelector));
        GC.KeepAlive(typeof(FileBrowserDialogHost));
    }
}

[AppRoot(Width = 1200, Height = 840, Title = "Files gallery")]
public sealed class App : Component
{
    private static readonly IReadOnlyList<FileSystemEntry> Entries = new List<FileSystemEntry>
    {
        new("Projects", @"C:\Users\Demo\Projects", true, 0, DateTime.Today.AddDays(-1)),
        new("Design assets", @"C:\Users\Demo\Design assets", true, 0, DateTime.Today.AddDays(-2)),
        new("README.md", @"C:\Users\Demo\README.md", false, 4_820, DateTime.Today.AddHours(-3), ".md"),
        new("product-spec.pdf", @"C:\Users\Demo\product-spec.pdf", false, 1_284_000, DateTime.Today.AddHours(-5), ".pdf"),
        new("hero-image.png", @"C:\Users\Demo\hero-image.png", false, 842_000, DateTime.Today.AddDays(-1), ".png"),
        new("FileBrowser.cs", @"C:\Users\Demo\FileBrowser.cs", false, 12_500, DateTime.Today.AddDays(-2), ".cs"),
        new("release-notes.txt", @"C:\Users\Demo\release-notes.txt", false, 3_100, DateTime.Today.AddDays(-4), ".txt"),
        new("brand-assets.zip", @"C:\Users\Demo\brand-assets.zip", false, 5_400_000, DateTime.Today.AddDays(-6), ".zip")
    };

    private static readonly IReadOnlyList<FileSystemEntry> Selected = new List<FileSystemEntry> { Entries[2] };

    private static readonly IReadOnlyList<DirectoryShortcut> Locations = new List<DirectoryShortcut>
    {
        new("Home", @"C:\Users\Demo"),
        new("Desktop", @"C:\Users\Demo\Desktop"),
        new("Documents", @"C:\Users\Demo\Documents"),
        new("Downloads", @"C:\Users\Demo\Downloads"),
        new("Pictures", @"C:\Users\Demo\Pictures")
    };

    private static readonly IReadOnlyList<FileSystemEntry> RecentDirectories = new List<FileSystemEntry>
    {
        Entries[0],
        Entries[1]
    };

    private static readonly IReadOnlyList<FileSystemEntry> RecentFiles = new List<FileSystemEntry>
    {
        Entries[2],
        Entries[3],
        Entries[4]
    };

    /// <inheritdoc/>
    public override Node Render()
    {
        var header = Div(new() { ClassName = "gallery-header" },
            H1(new() { ClassName = "gallery-title" }, Text("Files gallery")),
            P(new() { ClassName = "gallery-subtitle" },
                Text("A polished, reusable file browser built with Donkeh UI.")));

        var primary = Section("Primary",
            Div(new() { ClassName = "gallery-card" },
                CardLabel("FileBrowser"),
                Div(new() { ClassName = "preview-browser" },
                    _.C<FileBrowser, FileBrowserProps>(new FileBrowserProps()))));

        var dialogs = Section("Dialogs and selectors",
            Div(new() { ClassName = "gallery-grid gallery-grid-3" },
                Card("FileSelector",
                    Div(new() { ClassName = "demo-row" },
                        _.C<FileSelector, FileSelectorProps>(new FileSelectorProps
                        {
                            Text = "Select a file...",
                            Multiple = true
                        }))),
                Card("DirectorySelector",
                    Div(new() { ClassName = "demo-row" },
                        _.C<DirectorySelector, DirectorySelectorProps>(new DirectorySelectorProps
                        {
                            Text = "Select a directory..."
                        }))),
                Card("Dialog",
                    P(new() { ClassName = "gallery-subtitle" },
                        Text("Open a selector above to preview the modal dialog.")))));

        var toolbar = Card("Toolbar",
            Div(new() { ClassName = "preview-surface" },
                _.C<FileBrowserToolbar, FileBrowserToolbarProps>(new FileBrowserToolbarProps
                {
                    CurrentPath = @"C:\Users\Demo\Projects\OpenCreativeSuite",
                    CanGoBack = true,
                    CanGoForward = false,
                    CanGoUp = true,
                    ViewMode = FileBrowserViewMode.List
                })));

        var sidebar = Card("Sidebar",
            Div(new() { ClassName = "preview-surface preview-tall preview-sidebar" },
                _.C<FileBrowserSidebar, FileBrowserSidebarProps>(new FileBrowserSidebarProps
                {
                    CommonDirectories = Locations,
                    RecentDirectories = RecentDirectories,
                    RecentFiles = RecentFiles
                })));

        var list = Card("List",
            Div(new() { ClassName = "preview-surface preview-tall" },
                _.C<FileBrowserList, FileBrowserListProps>(new FileBrowserListProps
                {
                    Items = Entries,
                    Selection = Selected
                })));

        var grid = Card("Grid",
            Div(new() { ClassName = "preview-surface preview-tall" },
                _.C<FileBrowserGrid, FileBrowserGridProps>(new FileBrowserGridProps
                {
                    Items = Entries,
                    Selection = Selected
                })));

        var item = Card("Item",
            Div(new() { ClassName = "item-demo" },
                Div(new() { ClassName = "item-demo-list" },
                    Div(new() { ClassName = "preview-surface" },
                        _.C<FileBrowserItem, FileBrowserItemProps>(new FileBrowserItemProps
                        {
                            Entry = Entries[5],
                            Selected = true,
                            ViewMode = FileBrowserViewMode.List
                        }))),
                Div(new() { ClassName = "item-demo-grid" },
                    _.C<FileBrowserItem, FileBrowserItemProps>(new FileBrowserItemProps
                    {
                        Entry = Entries[0],
                        ViewMode = FileBrowserViewMode.Grid
                    }),
                    _.C<FileBrowserItem, FileBrowserItemProps>(new FileBrowserItemProps
                    {
                        Entry = Entries[4],
                        ViewMode = FileBrowserViewMode.Grid
                    }))));

        var context = Card("Context menu",
            Div(new() { ClassName = "preview-context" },
                _.C<FileBrowserContextMenu, FileBrowserContextMenuProps>(new FileBrowserContextMenuProps
                {
                    Open = true,
                    Inline = true,
                    Items = ContextItems(),
                    Request = new FileBrowserContextRequest
                    {
                        Selection = Selected,
                        Target = Entries[2],
                        CurrentPath = @"C:\Users\Demo"
                    }
                })));

        var blocks = Section("Building blocks",
            Div(new() { ClassName = "gallery-grid" },
                toolbar,
                sidebar,
                list,
                grid,
                item,
                context));

        return Div(new() { ClassName = "gallery" },
            Div(new() { ClassName = "gallery-page" },
                header,
                primary,
                dialogs,
                blocks),
            _.C(typeof(FileBrowserDialogHost)));
    }

    private static Node Section(string title, Node content)
    {
        return Div(new() { ClassName = "gallery-section" },
            Div(new() { ClassName = "gallery-section-title" }, Text(title)),
            content);
    }

    private static Node Card(string label, Node content)
    {
        return Div(new() { ClassName = "gallery-card" },
            CardLabel(label),
            content);
    }

    private static Node CardLabel(string label)
    {
        return Div(new() { ClassName = "gallery-card-label" }, Text(label));
    }

    private static IReadOnlyList<FileBrowserContextItem> ContextItems()
    {
        return new List<FileBrowserContextItem>
        {
            new() { Label = "Open", Icon = "▶" },
            new() { Label = "Copy path", Icon = "⧉", Shortcut = "Ctrl+C" },
            new() { IsSeparator = true },
            new() { Label = "Rename", Icon = "✎", Shortcut = "F2" },
            new() { Label = "Delete", Icon = "✕", Shortcut = "Del" },
            new() { IsSeparator = true },
            new() { Label = "Properties", Icon = "ℹ" }
        };
    }
}
