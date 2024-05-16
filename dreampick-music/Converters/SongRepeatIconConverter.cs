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
            AudioRepeatType.NO_REPEAT => FontAwesomeIcon.Undo,
            AudioRepeatType.REPEAT_QUEUE => FontAwesomeIcon.Retweet,
            AudioRepeatType.REPEAT_TRACK => FontAwesomeIcon.Repeat,
            _ => FontAwesomeIcon.MinusSquare
        };

        return FontAwesomeIcon.Pause;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}