using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dreampick_music;

public class PositionTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        TimeSpan item = (TimeSpan) value;

        
        return item != null ?  item.ToString(@"mm\:ss") : TimeSpan.FromSeconds(0).ToString(@"mm\:ss");
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}