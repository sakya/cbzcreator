using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CbzCreator.Lib.Models;

namespace CbzCreatorGui.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Info.Statuses status) {
            switch (status) {
                case Info.Statuses.Cancelled:
                    return new SolidColorBrush(App.GetStyleColor("DangerColor") ?? Colors.Red);
                case Info.Statuses.PublishingFinished:
                    return new SolidColorBrush(App.GetStyleColor("SuccessColor") ?? Colors.Green);
                case Info.Statuses.OnHiatus:
                    return new SolidColorBrush(App.GetStyleColor("WarningColor") ?? Colors.Orange);
                case Info.Statuses.Ongoing:
                    return new SolidColorBrush(App.GetStyleColor("InfoColor") ?? Colors.DodgerBlue);
            }
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}