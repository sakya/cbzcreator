using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CbzCreator.Lib.Models;
using CbzCreatorGui.Models;

namespace CbzCreatorGui
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<NameValue> statuses = new List<NameValue>()
            {
                new("Unknown", Info.Statuses.Unknown),
                new("Ongoing", Info.Statuses.Ongoing),
                new("Completed", Info.Statuses.Completed),
                new("Licensed", Info.Statuses.Licensed),
                new("Publishing finished", Info.Statuses.PublishingFinished),
                new("Cancelled", Info.Statuses.Cancelled),
                new("On hiatus", Info.Statuses.OnHiatus),
            };
            Status.Items = statuses;
        }

        private async void OnOpenFolderClick(object? sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog();
            var folder = await dlg.ShowAsync(this);
            if (!string.IsNullOrEmpty(folder)) {
                if (Equals(sender, InputButton)) {
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
                var res = await dlg.ShowDialog<Medium>(this);
                if (res != null) {
                    ComicTitle.Text = res.Title?.English ?? res.Title?.Romaji;
                    Authors.Text = res.Staff?.Author?.Node?.Name?.Full;
                    Artists.Text = res.Staff?.Artist?.Node?.Name?.Full;
                    Description.Text = StripHtml(res.Description);
                    Genres.Text = res.Genres != null ? string.Join(", ", res.Genres) : string.Empty;
                    Status.SelectedIndex = res.Status switch
                    {
                        "FINISHED" => (int)Info.Statuses.PublishingFinished,
                        "RELEASING" => (int)Info.Statuses.Ongoing,
                        "CANCELLED" => (int)Info.Statuses.Cancelled,
                        "HIATUS" => (int)Info.Statuses.OnHiatus,
                        _ => 0
                    };

                    CoverUrl.Text = !string.IsNullOrEmpty(res.CoverImage?.ExtraLarge) ? res.CoverImage?.ExtraLarge : string.Empty;
                }
            }
        }

        private async void OnCreateClick(object? sender, RoutedEventArgs e)
        {
            if (Validate()) {
                var info = new Info()
                {
                    Title = ComicTitle.Text,
                    Author = Authors.Text,
                    Artist = Artists.Text,
                    Description = Description.Text,
                    Genre = Genres.Text?.Split(new[] { ',' }).ToList(),
                    CoverUrl = !string.IsNullOrEmpty(CoverUrl.Text) ? new Uri(CoverUrl.Text) : null
                };
                if (Status.SelectedItem is NameValue value)
                    info.Status = (Info.Statuses)(value.Value ?? Info.Statuses.Unknown);

                var dlg = new LogWindow
                {
                    InputPath = InputPath.Text,
                    OutputPath = OutputPath.Text,
                    Info = info
                };
                await dlg.ShowDialog(this);
            }
        }

        private void OnComicTitleKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                OnSearchClick(null, new RoutedEventArgs());
        }

        private bool Validate()
        {
            var res = true;

            InputPath.Classes.Remove("Error");
            OutputPath.Classes.Remove("Error");
            ComicTitle.Classes.Remove("Error");
            CoverUrl.Classes.Remove("Error");

            if (string.IsNullOrEmpty(InputPath.Text?.Trim())) {
                InputPath.Classes.Add("Error");
                res = false;
            }

            if (string.IsNullOrEmpty(OutputPath.Text?.Trim())) {
                OutputPath.Classes.Add("Error");
                res = false;
            }

            if (string.IsNullOrEmpty(ComicTitle.Text?.Trim())) {
                ComicTitle.Classes.Add("Error");
                res = false;
            }

            if (!string.IsNullOrEmpty(CoverUrl.Text?.Trim())) {
                try {
                    var uri = new Uri(CoverUrl.Text);
                } catch (Exception) {
                    CoverUrl.Classes.Add("Error");
                    res = false;
                }
            }

            return res;
        }

        private static string? StripHtml(string? text)
        {
            if (text != null) {
                text = text.Replace("\r", string.Empty).Replace("\n", string.Empty);
                text = text.Replace("<br>", "\n");
                text = text.Replace("<b>", string.Empty);
                text = text.Replace("</b>", string.Empty);
                text = text.Replace("<i>", string.Empty);
                text = text.Replace("</i>", string.Empty);
            }

            return text;
        }
    }
}