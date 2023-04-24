using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CbzCreator.Lib;
using CbzCreator.Lib.Models;

namespace CbzCreatorGui;

public partial class LogWindow : Window
{
    private readonly CancellationTokenSource _tokenSource;
    private readonly StringBuilder _stringBuilder = new();

    public LogWindow()
    {
        InitializeComponent();

        _tokenSource = new CancellationTokenSource();
    }

    public string? InputPath { get; set; }
    public string? OutputPath { get; set; }
    public Info? Info { get; set; }

    protected override void OnOpened(EventArgs e)
    {
        if (Info == null || string.IsNullOrEmpty(InputPath) || string.IsNullOrEmpty(OutputPath))
            return;

        Task.Run(() => Creator.Create(Info, InputPath, OutputPath, _tokenSource.Token, LogMessage));
    }

    private void LogMessage(string message)
    {
        _stringBuilder.AppendLine(message);
        Dispatcher.UIThread.InvokeAsync(() => { Log.Text = _stringBuilder.ToString(); });
    }
}