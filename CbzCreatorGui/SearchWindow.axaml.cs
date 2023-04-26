using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CbzCreatorGui;

public partial class SearchWindow : Window
{
    private static HttpClient _client = new()
    {
        BaseAddress = new Uri("https://graphql.anilist.co/"),
    };

    public SearchWindow()
    {
        InitializeComponent();
    }

    public string? SearchTitle { get; set; }

    protected override void OnOpened(EventArgs e)
    {
        ComicTitle.Text = SearchTitle;
        OnSearchClick(null, new RoutedEventArgs());
    }

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
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                query = "\n query ($search: String) {\n Page(page: 1, perPage: 20) {\n media(search: $search, type: MANGA, format_not: NOVEL) {\n id\n title {\n romaji\n english\n }\n status\n description\n genres\n format\n coverImage { extraLarge }\n staff(perPage: 25) {\n edges {\n role\n node {\n id\n name { full }\n }\n }\n }\n }\n }\n }\n",
                variables = new {
                    search = title
                }
            }),
            Encoding.UTF8,
            "application/json");
        using HttpResponseMessage response = await _client.PostAsync(
            string.Empty,
            jsonContent);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var searchResult = JsonConvert.DeserializeObject<Models.SearchResult>(jsonResponse);

        List.Items = searchResult?.Data?.Page?.Media;
        return true;
    }

    private void OnListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        OkButton.IsEnabled = List.SelectedItems?.Count > 0;
    }

    private void OnComicTitleKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
            OnSearchClick(null, new RoutedEventArgs());
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        this.Close(List.SelectedItems?[0]);
    }
}