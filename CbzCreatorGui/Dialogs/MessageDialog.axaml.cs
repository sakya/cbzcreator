using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.SingleWindow.Abstracts;

namespace CbzCreatorGui.Dialogs;

public partial class MessageDialog : BaseDialog
{
    public MessageDialog()
    {
        InitializeComponent();

        VerticalAlignment = VerticalAlignment.Center;
        HorizontalAlignment = HorizontalAlignment.Center;
    }

    public string? Message
    {
        get => MessageText.Text;
        set => MessageText.Text = value;
    }

    public string? Title
    {
        get => TitleText.Text;
        set => TitleText.Text = value;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}