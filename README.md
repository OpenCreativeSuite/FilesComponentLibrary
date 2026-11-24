# Files

A reusable file-browser component library for [Donkeh](https://www.nuget.org/packages?q=Donkeh) apps. It ships a full browser widget, file/directory pickers, a modal dialog service, and the individual building blocks (toolbar, sidebar, list, grid, item, context menu) so you can compose your own views.

Every example below is shown twice:

- **Markup (`.cx`)** — the JSX-like syntax used in this repo's `*.cx` files (configured by `csx.json`).
- **Plain C#** — the builder API (`_.C<TComponent, TProps>(...)` and `HtmlHelper`).

## Install

```powershell
dotnet add package Files
```

The package targets `net10.0`. Component stylesheets are embedded in the package; make sure the `Files` assembly is loaded so the framework can discover its `[Stylesheet]` attributes. If your app never renders a component directly (for example, you only open dialogs through the service), force the assembly to load:

```csharp
using System.Runtime.CompilerServices;

internal static class LibraryLoader
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        GC.KeepAlive(typeof(OpenCreativeSuite.Files.FileBrowserDialogHost));
    }
}
```

## Setup: mount the dialog host

`FileSelector` and `DirectorySelector` publish their dialogs through `FileBrowserDialogService`. Mount **one** `FileBrowserDialogHost` near the root of your app; all dialogs then render in one fixed overlay instead of being trapped inside scrolled or transformed ancestors.

Markup (`.cx`):

```csharp
using OpenCreativeSuite.Files;

public override Node Render()
{
    return (
        <div className="app">
            <div>Your app content</div>
            <FileBrowserDialogHost />
        </div>
    );
}
```

Plain C#:

```csharp
using Donkeh.UILab;
using OpenCreativeSuite.Files;
using static Donkeh.UILab.HtmlHelper;
using static Donkeh.UILab._;

public override Node Render()
{
    return Div(new() { ClassName = "app" },
        /* ...your app... */
        _.C(typeof(FileBrowserDialogHost)));   // component without props
}
```

---

## Components

### `FileBrowser`

The complete browser: breadcrumb toolbar, sidebar, sortable list/grid, search, history (back/forward/up), context menu, and recent items. This is the component most apps need.

| Property | Type | Default | Notes |
| --- | --- | --- | --- |
| `InitialPath` | `string` | `""` (user profile) | Directory shown on first render |
| `InitialView` | `FileBrowserViewMode` | `List` | `List` or `Grid` |
| `AllowMultiSelect` | `bool` | `true` | Ctrl/Shift selection |
| `AllowFileSelection` | `bool` | `true` | Show/allow files |
| `AllowDirectorySelection` | `bool` | `true` | Show/allow folders |
| `AllowedExtensions` | `string[]` | empty | Allow-list, e.g. `".txt"` |
| `ExcludedExtensions` | `string[]` | empty | Deny-list |
| `OnOpen` | `Action<FileSystemEntry>?` | `null` | Fired when a file is opened (double-click) |
| `OnSelectionChanged` | `Action<IReadOnlyList<FileSystemEntry>>?` | `null` | Fired whenever selection changes |
| `FileSystemProvider` | `IFileSystemProvider?` | `LocalFileSystemProvider` | Swap in a virtual/remote file system |
| `RecentItemsService` | `RecentItemsService?` | `RecentItemsService.Default` | Shared recent-items store |
| `Theme` | `FileBrowserTheme?` | `null` | Custom theme |
| `DarkMode` | `bool` | `false` | Shortcut for `FileBrowserTheme.Dark` |

Markup (`.cx`):

```csharp
using OpenCreativeSuite.Files;

public override Node Render()
{
    return (
        <FileBrowser
            InitialPath={@"C:\Users\Demo"}
            InitialView={FileBrowserViewMode.List}
            AllowMultiSelect={true}
            AllowedExtensions={new[] { ".txt", ".md", ".pdf" }}
            OnOpen={(entry) => Console.WriteLine($"Open: {entry.FullPath}")}
            OnSelectionChanged={(items) => Console.WriteLine($"Selected: {items.Count}")}
            DarkMode={true} />
    );
}
```

Plain C#:

```csharp
using Donkeh.UILab;
using OpenCreativeSuite.Files;
using static Donkeh.UILab._;

public override Node Render()
{
    return _.C<FileBrowser, FileBrowserProps>(new FileBrowserProps
    {
        InitialPath = @"C:\Users\Demo",
        InitialView = FileBrowserViewMode.List,
        AllowMultiSelect = true,
        AllowedExtensions = new[] { ".txt", ".md", ".pdf" },
        OnOpen = entry => Console.WriteLine($"Open: {entry.FullPath}"),
        OnSelectionChanged = items => Console.WriteLine($"Selected: {items.Count}"),
        DarkMode = true
    });
}
```

With no props, both syntaxes are just the bare component:

```csharp
// .cx
<FileBrowser />

// C#
_.C<FileBrowser, FileBrowserProps>(new FileBrowserProps())
```

### `FileSelector`

A button showing the current selection. Clicking it opens a **file-only** dialog through `FileBrowserDialogService` (requires the `FileBrowserDialogHost` above).

| Property | Type | Default | Notes |
| --- | --- | --- | --- |
| `Text` | `string` | `"Select file"` | Placeholder label |
| `Multiple` | `bool` | `false` | Allow multiple files |
| `AllowedExtensions` / `ExcludedExtensions` | `string[]` | empty | File-type filters |
| `InitialPath` / `InitialView` | `string` / `FileBrowserViewMode` | `""` / `List` | Dialog starting location |
| `FileSystemProvider` / `RecentItemsService` / `Theme` / `DarkMode` | see `FileBrowser` | — | Forwarded to the dialog |
| `OnSelectionChanged` | `Action<IReadOnlyList<FileSystemEntry>>?` | `null` | Called after confirmation |

Markup (`.cx`):

```csharp
<FileSelector
    Text={"Select a file..."}
    Multiple={true}
    AllowedExtensions={new[] { ".png", ".jpg" }}
    OnSelectionChanged={(items) => Console.WriteLine(string.Join(", ", items.Select(i => i.Name)))} />
```

Plain C#:

```csharp
_.C<FileSelector, FileSelectorProps>(new FileSelectorProps
{
    Text = "Select a file...",
    Multiple = true,
    AllowedExtensions = new[] { ".png", ".jpg" },
    OnSelectionChanged = items => Console.WriteLine(string.Join(", ", items.Select(i => i.Name)))
})
```

### `DirectorySelector`

Same idea as `FileSelector`, but **folder-only**. The button displays the selected folder's full path.

Markup (`.cx`):

```csharp
<DirectorySelector
    Text={"Select a directory..."}
    Multiple={false}
    InitialPath={@"C:\Users\Demo"}
    OnSelectionChanged={(items) => Console.WriteLine(items[0].FullPath)} />
```

Plain C#:

```csharp
_.C<DirectorySelector, DirectorySelectorProps>(new DirectorySelectorProps
{
    Text = "Select a directory...",
    Multiple = false,
    InitialPath = @"C:\Users\Demo",
    OnSelectionChanged = items => Console.WriteLine(items[0].FullPath)
})
```

### Dialogs: `FileBrowserDialogService`, `FileBrowserDialog`, `FileBrowserDialogHost`

Open a modal directly from code with `FileBrowserDialogService.Open`. The host listens on `FileBrowserDialogService.Requested` and renders a `FileBrowserDialog`.

| `FileBrowserDialogRequest` property | Notes |
| --- | --- |
| `Title` | Header text (default `"Select"`) |
| `InitialPath`, `InitialView` | Starting directory and view |
| `AllowMultiSelect`, `AllowFileSelection`, `AllowDirectorySelection` | Selection rules |
| `AllowedExtensions`, `ExcludedExtensions` | Filters |
| `FileSystemProvider`, `RecentItemsService`, `Theme`, `DarkMode` | Forwarded to the inner browser |
| `OnConfirm` | `Action<IReadOnlyList<FileSystemEntry>>` — selection |
| `OnCancel` | `Action` |

C# (service calls are plain C# in both syntaxes):

```csharp
using OpenCreativeSuite.Files;

FileBrowserDialogService.Open(new FileBrowserDialogRequest
{
    Title = "Open project",
    InitialPath = @"C:\Users\Demo",
    AllowMultiSelect = true,
    AllowedExtensions = new[] { ".sln", ".csproj" },
    OnConfirm = items => Console.WriteLine($"Chose {items.Count} project(s)"),
    OnCancel = () => Console.WriteLine("Cancelled")
});
```

`FileBrowserDialog` can also be rendered directly if you want to control `Open` yourself:

Markup (`.cx`):

```csharp
<FileBrowserDialog
    Open={dialogOpen}
    Title={"Save as"}
    AllowFileSelection={true}
    AllowDirectorySelection={false}
    OnConfirm={(items) => Save(items[0])}
    OnCancel={() => SetDialogOpen(false)} />
```

Plain C#:

```csharp
_.C<FileBrowserDialog, FileBrowserDialogProps>(new FileBrowserDialogProps
{
    Open = dialogOpen,
    Title = "Save as",
    AllowFileSelection = true,
    AllowDirectorySelection = false,
    OnConfirm = items => Save(items[0]),
    OnCancel = () => SetDialogOpen(false)
})
```

### Building blocks

Use these when you need just one piece of the browser.

#### `FileBrowserToolbar`

Navigation buttons, breadcrumbs, search box, and List/Grid toggle.

| Property | Type | Notes |
| --- | --- | --- |
| `CurrentPath` | `string` | Drives breadcrumbs |
| `CanGoBack`, `CanGoForward`, `CanGoUp` | `bool` | Enable/disable nav buttons |
| `OnBack`, `OnForward`, `OnUp`, `OnRefresh` | `Action?` | Nav callbacks |
| `OnNavigate` | `Action<string>?` | Breadcrumb click (receives full path) |
| `SearchQuery` | `string` | Search input value |
| `OnSearchChanged` | `Action<string>?` | Search input callback |
| `ViewMode` | `FileBrowserViewMode` | Active toggle |
| `OnViewModeChanged` | `Action<FileBrowserViewMode>?` | Toggle callback |
| `Theme` | `FileBrowserTheme?` | Optional theme |

Markup (`.cx`):

```csharp
<FileBrowserToolbar
    CurrentPath={@"C:\Users\Demo\Projects"}
    CanGoBack={true}
    CanGoUp={true}
    ViewMode={FileBrowserViewMode.List}
    OnBack={GoBack}
    OnUp={GoUp}
    OnSearchChanged={(text) => Search(text)}
    OnViewModeChanged={(mode) => SetMode(mode)}
    OnNavigate={(path) => Navigate(path)} />
```

Plain C#:

```csharp
_.C<FileBrowserToolbar, FileBrowserToolbarProps>(new FileBrowserToolbarProps
{
    CurrentPath = @"C:\Users\Demo\Projects",
    CanGoBack = true,
    CanGoUp = true,
    ViewMode = FileBrowserViewMode.List,
    OnBack = GoBack,
    OnUp = GoUp,
    OnSearchChanged = text => Search(text),
    OnViewModeChanged = mode => SetMode(mode),
    OnNavigate = path => Navigate(path)
})
```

#### `FileBrowserSidebar`

Locations + recent directories/files, from a `RecentItemsService`.

Markup (`.cx`):

```csharp
<FileBrowserSidebar
    CommonDirectories={new[] { new DirectoryShortcut("Home", @"C:\Users\Demo") }}
    RecentDirectories={recent.RecentDirectories}
    RecentFiles={recent.RecentFiles}
    OnNavigate={(path) => Navigate(path)}
    OnOpenRecent={(entry) => Open(entry)}
    OnClearRecent={() => recent.ClearAll()} />
```

Plain C#:

```csharp
_.C<FileBrowserSidebar, FileBrowserSidebarProps>(new FileBrowserSidebarProps
{
    CommonDirectories = new[] { new DirectoryShortcut("Home", @"C:\Users\Demo") },
    RecentDirectories = recent.RecentDirectories,
    RecentFiles = recent.RecentFiles,
    OnNavigate = path => Navigate(path),
    OnOpenRecent = entry => Open(entry),
    OnClearRecent = () => recent.ClearAll()
})
```

#### `FileBrowserList` / `FileBrowserGrid`

Read-only views over entries you supply. `List` adds sortable column headers (`name`, `type`, `size`, `date`); `Grid` is a plain icon grid. Both share: `Items`, `Selection`, `OnItemClicked(entry, ctrl, shift)`, `OnItemDoubleClicked(entry)`, `OnItemContextMenu(event, entry)`, `Theme`. `List` also has `SortColumn`, `SortAscending`, and `OnSort(column)`.

Markup (`.cx`):

```csharp
<FileBrowserList
    Items={entries}
    Selection={selection}
    SortColumn={"name"}
    SortAscending={true}
    OnSort={(column) => Sort(column)}
    OnItemClicked={(entry, ctrl, shift) => Select(entry, ctrl, shift)}
    OnItemDoubleClicked={(entry) => Open(entry)} />

<FileBrowserGrid
    Items={entries}
    Selection={selection}
    OnItemClicked={(entry, ctrl, shift) => Select(entry, ctrl, shift)}
    OnItemDoubleClicked={(entry) => Open(entry)} />
```

Plain C#:

```csharp
_.C<FileBrowserList, FileBrowserListProps>(new FileBrowserListProps
{
    Items = entries,
    Selection = selection,
    SortColumn = "name",
    SortAscending = true,
    OnSort = column => Sort(column),
    OnItemClicked = (entry, ctrl, shift) => Select(entry, ctrl, shift),
    OnItemDoubleClicked = entry => Open(entry)
}),

_.C<FileBrowserGrid, FileBrowserGridProps>(new FileBrowserGridProps
{
    Items = entries,
    Selection = selection,
    OnItemClicked = (entry, ctrl, shift) => Select(entry, ctrl, shift),
    OnItemDoubleClicked = entry => Open(entry)
})
```

#### `FileBrowserItem`

A single row/tile. Useful for custom views. `Entry` is required; `OnItemClicked(ctrl, shift)` handles Ctrl/Shift clicks, `OnDoubleClick` fires on a double click, and `OnClick` is a plain click.

Markup (`.cx`):

```csharp
<FileBrowserItem
    Entry={new FileSystemEntry("notes.md", @"C:\Users\Demo\notes.md", false, 1024, DateTime.Now, ".md")}
    Selected={true}
    ViewMode={FileBrowserViewMode.List}
    OnItemClicked={(ctrl, shift) => Select(ctrl, shift)}
    OnDoubleClick={() => Open()} />
```

Plain C#:

```csharp
_.C<FileBrowserItem, FileBrowserItemProps>(new FileBrowserItemProps
{
    Entry = new FileSystemEntry("notes.md", @"C:\Users\Demo\notes.md", false, 1024, DateTime.Now, ".md"),
    Selected = true,
    ViewMode = FileBrowserViewMode.List,
    OnItemClicked = (ctrl, shift) => Select(ctrl, shift),
    OnDoubleClick = () => Open()
})
```

#### `FileBrowserContextMenu`

A positioned menu, optionally inline (for embedding in a custom layout). Items are `FileBrowserContextItem` objects; `OnClose` fires on outside click or item click.

Markup (`.cx`):

```csharp
<FileBrowserContextMenu
    Open={menuOpen}
    X={x}
    Y={y}
    Items={items}
    Request={request}
    OnClose={() => SetMenuOpen(false)} />
```

Plain C#:

```csharp
_.C<FileBrowserContextMenu, FileBrowserContextMenuProps>(new FileBrowserContextMenuProps
{
    Open = menuOpen,
    X = x,
    Y = y,
    Items = items,
    Request = request,
    OnClose = () => SetMenuOpen(false)
})
```

---

## Models

| Type | Purpose |
| --- | --- |
| `FileSystemEntry` | `record(Name, FullPath, IsDirectory, Size = 0, Modified = default, Extension = "", MimeType = null)` |
| `DirectoryShortcut` | `record(Label, Path)` — sidebar location |
| `FileBrowserViewMode` | `List` / `Grid` |
| `FileBrowserTheme` | Theme colors with `Light` / `Dark` presets and `ToCssVariables()` |
| `FileBrowserOptions` | Plain settings bag (`InitialPath`, view, selection flags, filters, `MaxRecentItems`) |
| `FileBrowserDialogRequest` | Dialog descriptor used by `FileBrowserDialogService` |
| `FileBrowserContextItem` / `FileBrowserContextRequest` / `FileBrowserContextMenuState` | Context menu data |

## Services

### `IFileSystemProvider` / `LocalFileSystemProvider`

`FileBrowser` uses `LocalFileSystemProvider` by default. Implement `IFileSystemProvider` to browse something else (virtual folders, cloud storage, an in-memory test file system) and pass it via `FileSystemProvider`.

```csharp
using OpenCreativeSuite.Files;

public sealed class MemoryFileSystemProvider : IFileSystemProvider
{
    public Task<IReadOnlyList<FileSystemEntry>> EnumerateAsync(string path, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<FileSystemEntry>>(new[]
        {
            new FileSystemEntry("readme.md", "mem:///readme.md", false, 42, DateTime.Today, ".md"),
            new FileSystemEntry("Assets", "mem:///Assets", true)
        });

    public Task<FileSystemEntry?> GetEntryAsync(string path, CancellationToken ct = default)
        => Task.FromResult<FileSystemEntry?>(null);
    public Task<bool> FileExistsAsync(string path, CancellationToken ct = default) => Task.FromResult(false);
    public Task<bool> DirectoryExistsAsync(string path, CancellationToken ct = default) => Task.FromResult(true);
    public string GetParentPath(string path) => "mem:///";
    public bool IsRoot(string path) => path == "mem:///";
    public IReadOnlyList<DirectoryShortcut> GetCommonDirectories()
        => new[] { new DirectoryShortcut("Memory", "mem:///") };
    public Task DeleteAsync(string path, CancellationToken ct = default) => Task.CompletedTask;
    public Task RenameAsync(string path, string newName, CancellationToken ct = default) => Task.CompletedTask;
    public Task CopyAsync(string sourcePath, string destinationPath, CancellationToken ct = default) => Task.CompletedTask;
    public Task CreateDirectoryAsync(string path, CancellationToken ct = default) => Task.CompletedTask;
}
```

Then use it in either syntax:

```csharp
// .cx
<FileBrowser FileSystemProvider={new MemoryFileSystemProvider()} InitialPath={"mem:///"} />

// C#
_.C<FileBrowser, FileBrowserProps>(new FileBrowserProps
{
    FileSystemProvider = new MemoryFileSystemProvider(),
    InitialPath = "mem:///"
})
```

### `RecentItemsService`

Tracks recently opened files/directories. `RecentItemsService.Default` is shared; pass your own instance to keep state isolated. Members: `RecentFiles`, `RecentDirectories`, `AddFile`, `AddDirectory`, `Remove`, `ClearFiles`, `ClearDirectories`, `ClearAll`, `Changed`, `MaxItems` (default 20).

```csharp
var recent = new RecentItemsService { MaxItems = 10 };
recent.AddFile(new FileSystemEntry("notes.md", @"C:\Users\Demo\notes.md", false, 1024));
recent.Changed += () => Console.WriteLine("Recent items changed");
```

### `FileBrowserContextRegistry`

Add your own entries to the browser's right-click menu, scoped by an owner key. Entries are filtered by `Applies()` (extension, MIME type, folder/file, selection count, predicate, visibility) and sorted by `Order`.

```csharp
FileBrowserContextRegistry.Default.Register("my-app", new[]
{
    new FileBrowserContextItem
    {
        Label = "Open in editor",
        Icon = "✎",
        Shortcut = "Ctrl+E",
        Extensions = new[] { ".cs", ".txt" },
        Order = 10,
        OnClick = req => Console.WriteLine(req.Selection[0].FullPath)
    }
});

// Later:
FileBrowserContextRegistry.Default.Unregister("my-app");
```

`FileBrowserClipboard.Text` is a simple in-app clipboard used by the built-in "Copy path" / "Copy filename" menu items.

## Theming

Use the built-in presets, flip `DarkMode`, or supply your own theme. All themes are applied as CSS custom properties (`--file-browser-*`), so any color can be overridden.

```csharp
var theme = new FileBrowserTheme
{
    Accent = "#7c3aed",
    Background = "#fafafa",
    SurfaceSelected = "#ede9fe"
};
```

```csharp
// .cx
<FileBrowser Theme={theme} />

// C#
_.C<FileBrowser, FileBrowserProps>(new FileBrowserProps { Theme = theme })
```

`DarkMode = true` is shorthand for `Theme = FileBrowserTheme.Dark`; an explicit `Theme` wins over `DarkMode`.

## Preview

The `test-container` project is a live gallery of every public component.

```powershell
donkeh enable-feature hot-reload
donkeh watch
```

It discovers every public component in `src/` and renders it in a gallery. Supported C# edits and CSS edits update the running preview; unsupported C# edits restart the app.

## Publish

```powershell
dotnet pack src/Files.csproj -c Release
```

The library is marked packable, carries the component stylesheets as embedded resources, and embeds this README so it is displayed on the NuGet package page.
