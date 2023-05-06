using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.SingleWindow.Abstracts;
using Octokit;

namespace CbzCreatorGui.Dialogs;

public partial class UpdateDialog : BaseDialog
{
    private readonly Release? _release;
    public UpdateDialog()
    {
        InitializeComponent();
    }

    public UpdateDialog(Release release, string changelog)
    {
        InitializeComponent();

        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;

        _release = release;
        Version.Text = release.TagName;
        Description.Text = changelog;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        var url = _release!.HtmlUrl;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            //https://stackoverflow.com/a/2796367/241446
            using var proc = new Process { StartInfo = { UseShellExecute = true, FileName = url } };
            proc.Start();
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Process.Start("x-www-browser", url);
        } else {
            Process.Start("open", url);
        }

        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}