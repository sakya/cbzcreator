using System;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace CbzCreatorGui.Converters;

public class FormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var format = value as string;
        if (!string.IsNullOrEmpty(format)) {
            var sb = new StringBuilder();
            var first = true;
            for (var i = 0; i < format.Length; i++) {
                var c = format[i];
                if (c == '_')
                    c = ' ';
                sb.Append(first ? char.ToUpper(c) : char.ToLower(c));
                first = c == ' ';
            }

            format = sb.ToString();
        }

        return format;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}