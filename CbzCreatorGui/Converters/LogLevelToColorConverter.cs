using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CbzCreator.Lib;

namespace CbzCreatorGui.Converters;

public class LogLevelToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var level = (Creator.LogLevel?)value;
        if (level != null) {
            Color? c = null;
            switch (level.Value) {
                case Creator.LogLevel.Debug:
                    c = App.GetStyleColor("DebugColor");
                    break;
                case Creator.LogLevel.Info:
                    c = App.GetStyleColor("InfoColor");
                    break;
                case Creator.LogLevel.Warning:
                    c = App.GetStyleColor("WarningColor");
                    break;
                case Creator.LogLevel.Error:
                    c = App.GetStyleColor("DangerColor");
                    break;
            }

            if (c != null)
                return new SolidColorBrush(c.Value);
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}