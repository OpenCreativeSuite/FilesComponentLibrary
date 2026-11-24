# Files

A Donkeh component library with a live test container.

## Preview

```powershell
donkeh enable-feature hot-reload
donkeh watch
```

The container discovers every public component in `src/` and renders it
in a gallery. Supported C# edits and CSS edits update the running
preview; unsupported C# edits restart the app.

## Publish

```powershell
dotnet pack src/Files.csproj -c Release
```

The library is marked packable and carries the component stylesheets as
embedded resources.