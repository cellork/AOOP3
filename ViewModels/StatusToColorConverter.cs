using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using AOOP3.Models;

namespace AOOP3.ViewModels;

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FlightStatus status)
        {
            return status switch
            {
                FlightStatus.Scheduled => new SolidColorBrush(Color.Parse("#B3E5FC")), // Light Blue
                FlightStatus.Landed => new SolidColorBrush(Color.Parse("#C8E6C9")), // Light Green
                FlightStatus.Delayed => new SolidColorBrush(Color.Parse("#FFECB3")), // Light Orange
                FlightStatus.Cancelled => new SolidColorBrush(Color.Parse("#FFCCBC")), // Light Red
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
