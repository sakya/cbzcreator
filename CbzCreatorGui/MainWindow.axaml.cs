using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CbzCreatorGui
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnOpenFolderClick(object? sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog();
            var folder = await dlg.ShowAsync(this);
            if (!string.IsNullOrEmpty(folder)) {
                if (sender == InputButton) {
                    InputPath.Text = folder;
                } else {
                    OutputPath.Text = folder;
                }
            }
        }

        private async void OnSearchClick(object? sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty((ComicTitle.Text?.Trim()))) {
                var dlg = new SearchWindow
                {
                    SearchTitle = ComicTitle.Text
                };
                await dlg.ShowDialog(this);
            }
        }

        private void OnCreateClick(object? sender, RoutedEventArgs e)
        {

        }
    }
}