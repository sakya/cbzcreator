using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace CbzCreatorGui
{
    public class App : Application
    {
        private Window? MainWindow { get; set; }

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
            if (Current is App { MainWindow: not null } app) {
                app.MainWindow.LayoutManager.ExecuteLayoutPass();
            }
        }

        public static Color? GetStyleColor(string name)
        {
            if (Current is App app && app.MainWindow != null) {
                var resource = app.MainWindow.FindResource(name);
                if (resource is Color col)
                    return col;
            }
            return null;
        }
    }
}