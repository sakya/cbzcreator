using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace CbzCreatorGui
{
    public partial class App : Application
    {
        public Avalonia.Controls.Window? MainWindow { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow();
                this.MainWindow = desktop.MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static void UpdateLayout()
        {
            if (Current is App app && app.MainWindow != null) {
                app.MainWindow.LayoutManager.ExecuteLayoutPass();
            }
        }

        public static Color? GetStyleColor(string name)
        {
            if (Current is App app && app.MainWindow != null) {
                var resource = app.MainWindow.FindResource(name);
                if (resource is Color col)
                    return (Color)resource;
            }
            return null;
        }
    }
}