using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CbzCreator.Lib;
using CbzCreator.Lib.Models;

namespace CbzCreatorGui;

public partial class LogWindow : Window
{
    private bool _running;
    private readonly CancellationTokenSource _tokenSource;
    private readonly StringBuilder _stringBuilder = new();

    public LogWindow()
    {
        InitializeComponent();

        _tokenSource = new CancellationTokenSource();
    }

    public string? InputPath { get; init; }
    public string? OutputPath { get; init; }
    public Info? Info { get; init; }

    protected override void OnOpened(EventArgs e)
    {
        if (Info == null || string.IsNullOrEmpty(InputPath) || string.IsNullOrEmpty(OutputPath))
            return;

        Task.Run(() =>
        {
            try {
                Creator.Create(Info, InputPath, OutputPath, _tokenSource.Token, LogMessage);
            } catch (Exception ex) {
                LogMessage(Creator.LogLevel.Error, ex.Message);
            }
            _running = false;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Button.Classes.Add("Accent");
                ButtonText.Text = "Close";
            });
        });
    }

    private void LogMessage(Creator.LogLevel level, string message)
    {
        if (level < Creator.LogLevel.Info)
            return;

        var prefix = level switch
        {
            Creator.LogLevel.Info => "[INF]",
            Creator.LogLevel.Warning => "[WRN]",
            Creator.LogLevel.Error => "[ERR]",
            _ => string.Empty
        };
        _stringBuilder.AppendLine($"{prefix}{message}");
        Dispatcher.UIThread.InvokeAsync(() => { Log.Text = _stringBuilder.ToString(); });
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (!_running) {
            Close();
        } else {
            _tokenSource.Cancel();
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_running)
            e.Cancel = true;
    }
}