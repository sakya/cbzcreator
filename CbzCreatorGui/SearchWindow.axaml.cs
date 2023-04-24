using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CbzCreatorGui;

public partial class SearchWindow : Window
{
    public SearchWindow()
    {
        InitializeComponent();
    }

    public string? SearchTitle { get; set; }

    private async void OnSearchClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ComicTitle.Text?.Trim())) {
            SearchButton.IsEnabled = false;
            await Search(ComicTitle.Text);
            SearchButton.IsEnabled = true;
        }
    }

    private async Task<bool> Search(string title)
    {
        return true;
    }
}