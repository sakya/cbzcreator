using System;
using System.Reflection;
using Avalonia.SingleWindow;
using Avalonia.Threading;
using CbzCreatorGui.Pages;

namespace CbzCreatorGui
{
    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                ExtendClientAreaToDecorationsHint = true;
                ExtendClientAreaTitleBarHeightHint = -1;
                ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
            }

            WindowTitle = $"CBZ creator - v{Assembly.GetExecutingAssembly().GetName().Version!.ToString()}";
            Container = ContainerGrid;
        }

        protected override async void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            await NavigateTo(new MainPage());

            DispatcherTimer.RunOnce(async () =>
            {
                var updater = new Utils.AutoUpdater();
                await updater.CheckForUpdate(this);
            }, TimeSpan.FromSeconds(2), DispatcherPriority.Background);
        }

        protected override void PageChanged()
        {
            TitleBar.CanGoBack = CanNavigateBack;
        }
    }
}