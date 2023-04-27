using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.SingleWindow.Abstracts;
using Avalonia.Threading;
using CbzCreator.Lib;
using CbzCreator.Lib.Models;
using CbzCreatorGui.Models;

namespace CbzCreatorGui.Dialogs;

public partial class LogDialog : BaseDialog
{
    private bool _running;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ObservableCollection<LogMessage> _log = new();

    public LogDialog()
    {
        InitializeComponent();

        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalAlignment = HorizontalAlignment.Stretch;

        LogList.Items = _log;
        _tokenSource = new CancellationTokenSource();
        Closing += OnClosing;
    }

    public string? InputPath { get; init; }
    public string? OutputPath { get; init; }
    public Info? Info { get; init; }

    protected override void Opened()
    {
        if (Info == null || string.IsNullOrEmpty(InputPath) || string.IsNullOrEmpty(OutputPath))
            return;

        Task.Run(() =>
        {
            _running = true;
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
            Creator.LogLevel.Debug => "[DBG]",
            Creator.LogLevel.Info => "[INF]",
            Creator.LogLevel.Warning => "[WRN]",
            Creator.LogLevel.Error => "[ERR]",
            _ => string.Empty
        };

        Dispatcher.UIThread.Post(() =>
        {
            while (_log.Count > 200)
                _log.RemoveAt(0);

            _log.Add(new LogMessage()
            {
                Level = level,
                Message =$"{prefix}{message}"
            });
            App.UpdateLayout();
            Log.ScrollToEnd();
        });
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (!_running) {
            Close();
        } else {
            _tokenSource.Cancel();
        }
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_running)
            e.Cancel = true;
    }
}