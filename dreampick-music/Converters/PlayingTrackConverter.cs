using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FontAwesome.WPF;

namespace dreampick_music.Converters;

public class PlayingTrackConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = (string)value;
        var isPlaying = false;

        try
        {
            isPlaying = PlayerVm.Instance.CurrentTrack.ID == str && PlayerVm.Instance.SongState == MediaState.Play;
        }
        catch {}
        

        return isPlaying ? FontAwesomeIcon.Pause : FontAwesomeIcon.Play;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}