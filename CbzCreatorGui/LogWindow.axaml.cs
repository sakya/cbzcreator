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
                LogMessage($"Error: {ex.Message}");
            }
            _running = false;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Button.Classes.Add("Accent");
                ButtonText.Text = "Close";
            });
        });
    }

    private void LogMessage(string message)
    {
        _stringBuilder.AppendLine(message);
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