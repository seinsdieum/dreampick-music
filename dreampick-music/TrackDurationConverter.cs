using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dreampick_music;

public class TrackDurationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Duration item = (Duration)value;
        
        return item.HasTimeSpan ? item.TimeSpan.Seconds : 0;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}