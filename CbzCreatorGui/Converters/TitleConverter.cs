using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CbzCreatorGui.Models;

namespace CbzCreatorGui.Converters;

public class TitleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var title = value as Title;
        if (title == null)
            return string.Empty;

        return title.English ?? title.Romaji ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}