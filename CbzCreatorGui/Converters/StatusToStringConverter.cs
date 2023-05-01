using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CbzCreator.Lib.Models;

namespace CbzCreatorGui.Converters;

public class StatusToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Info.Statuses status) {
            switch (status) {
                case Info.Statuses.Cancelled:
                    return "Cancelled";
                case Info.Statuses.PublishingFinished:
                    return "Publishing finished";
                case Info.Statuses.OnHiatus:
                    return "On hiatus";
                case Info.Statuses.Ongoing:
                    return "Ongoing";
            }
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}