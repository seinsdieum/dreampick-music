using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dreampick_music.Converters;

public class ThirdToSecondColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        return value is true ? App.Current.Resources["SecondBorderRoot"] : App.Current.Resources["ThirdBorderRoot"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}