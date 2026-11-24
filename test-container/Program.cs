using Donkeh.Hosting;
using Donkeh.UILab;
using static Donkeh.UILab.HtmlHelper;
using static Donkeh.UILab._;
using System.Runtime.CompilerServices;
using OpenCreativeSuite.Components;

[assembly: Donkeh.UILab.Stylesheet("Styles/gallery.css")]

namespace Files.TestContainer;

internal static class LibraryLoader
{
    [ModuleInitializer]
    internal static void Initialize() => GC.KeepAlive(typeof(FileSelector));
}

[AppRoot(Width = 1200, Height = 800, Title = "Files gallery")]
public sealed class App : Component
{
    /// <inheritdoc/>
    public override Node Render() =>
        Div(new() { ClassName = "gallery" },
            Div(new() { ClassName = "gallery-header" },
                H1(new() { ClassName = "gallery-title" }, Text("Files gallery")),
                P(new() { ClassName = "gallery-subtitle" },
                    Text("Live preview for every component in the library."))),
            Div(new() { ClassName = "gallery-grid" },
                Each(ComponentTypes(), type =>
                    Div(new() { ClassName = "gallery-item" },
                        H2(new() { ClassName = "gallery-name" }, Text(type.Name)),
                        Div(new() { ClassName = "gallery-preview" }, C(type))))));

    /// <summary>Finds every public, constructible component in the library.</summary>
    private static Type[] ComponentTypes() =>
        typeof(FileSelector).Assembly
            .GetTypes()
            .Where(type => typeof(Component).IsAssignableFrom(type)
                && !type.IsAbstract
                && type.GetConstructor(Type.EmptyTypes) is not null)
            .OrderBy(type => type.Name, StringComparer.Ordinal)
            .ToArray();
}