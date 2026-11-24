namespace OpenCreativeSuite.Files
{
    public sealed class FileBrowserTheme
    {
        public string Background { get; set; } = "#ffffff";
        public string Surface { get; set; } = "#ffffff";
        public string SurfaceHover { get; set; } = "#f3f4f6";
        public string SurfaceSelected { get; set; } = "#dbeafe";
        public string Border { get; set; } = "#d1d5db";
        public string Text { get; set; } = "#111827";
        public string TextMuted { get; set; } = "#6b7280";
        public string Accent { get; set; } = "#2563eb";
        public string Icon { get; set; } = "#6b7280";
        public string SidebarBackground { get; set; } = "#f0f2f5";
        public string ToolbarBackground { get; set; } = "#ffffff";
        public string ContextBackground { get; set; } = "#ffffff";
        public string DialogBackground { get; set; } = "#ffffff";
        public string Focus { get; set; } = "#2563eb";

        public static FileBrowserTheme Light { get; } = new FileBrowserTheme();

        public static FileBrowserTheme Dark { get; } = new FileBrowserTheme
        {
            Background = "#0f172a",
            Surface = "#1e293b",
            SurfaceHover = "#334155",
            SurfaceSelected = "#1e3a8a",
            Border = "#334155",
            Text = "#e2e8f0",
            TextMuted = "#94a3b8",
            Accent = "#60a5fa",
            Icon = "#94a3b8",
            SidebarBackground = "#0b1220",
            ToolbarBackground = "#111827",
            ContextBackground = "#1e293b",
            DialogBackground = "#0f172a",
            Focus = "#60a5fa"
        };

        public string ToCssVariables()
        {
            return string.Join(";", new[]
            {
                $"--file-browser-background:{Background}",
                $"--file-browser-surface:{Surface}",
                $"--file-browser-surface-hover:{SurfaceHover}",
                $"--file-browser-surface-selected:{SurfaceSelected}",
                $"--file-browser-border:{Border}",
                $"--file-browser-text:{Text}",
                $"--file-browser-text-muted:{TextMuted}",
                $"--file-browser-accent:{Accent}",
                $"--file-browser-icon:{Icon}",
                $"--file-browser-sidebar-background:{SidebarBackground}",
                $"--file-browser-toolbar-background:{ToolbarBackground}",
                $"--file-browser-context-background:{ContextBackground}",
                $"--file-browser-dialog-background:{DialogBackground}",
                $"--file-browser-focus:{Focus}"
            });
        }
    }
}