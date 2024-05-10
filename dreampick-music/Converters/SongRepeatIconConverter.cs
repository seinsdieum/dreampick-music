using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FontAwesome.WPF;

namespace dreampick_music.Converters;

public class SongRepeatIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        return value switch
        {
            AudioRepeatType.NO_REPEAT => FontAwesomeIcon.ArrowRight,
            AudioRepeatType.REPEAT_QUEUE => FontAwesomeIcon.Repeat,
            AudioRepeatType.REPEAT_TRACK => FontAwesomeIcon.ArrowCircleRight,
            _ => FontAwesomeIcon.MinusSquare
        };

        return FontAwesomeIcon.Pause;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}