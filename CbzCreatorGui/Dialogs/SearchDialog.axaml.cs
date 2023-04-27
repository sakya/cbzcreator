using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.SingleWindow.Abstracts;
using Newtonsoft.Json;

namespace CbzCreatorGui.Dialogs;

public partial class SearchDialog : BaseDialog
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("https://graphql.anilist.co/"),
    };

    public SearchDialog()
    {
        InitializeComponent();
        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }

    public string? SearchTitle { get; set; }

    protected override void Opened()
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
        } else {
            List.Items = null;
        }
    }

    private async Task<bool> Search(string title)
    {
        using StringContent jsonContent = new(
            JsonConvert.SerializeObject(new
            {
                query = "\n query ($search: String) {\n Page(page: 1, perPage: 20) {\n media(search: $search, type: MANGA, format_not: NOVEL) {\n id\n title {\n romaji\n english\n }\n status\n description\n genres\n format\n coverImage { extraLarge }\n staff(perPage: 25) {\n edges {\n role\n node {\n id\n name { full }\n }\n }\n }\n }\n }\n }\n",
                variables = new {
                    search = title
                }
            }),
            Encoding.UTF8,
            "application/json");
        using HttpResponseMessage response = await Client.PostAsync(
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