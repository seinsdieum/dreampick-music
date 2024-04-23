using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dreampick_music.Utilities;

public class StatementVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool item = (bool)value;
        
        return item ? Visibility.Visible : Visibility.Collapsed;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}