using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FontAwesome.WPF;

namespace dreampick_music;

public class SongStateIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var item = value is MediaState state ? state : MediaState.Pause;

        return item == MediaState.Play ? FontAwesomeIcon.Pause : FontAwesomeIcon.Play;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}