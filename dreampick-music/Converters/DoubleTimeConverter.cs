using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dreampick_music;

public class DoubleTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        TimeSpan item = TimeSpan.FromSeconds((double)value);

        return item.ToString(@"mm\:ss");
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}