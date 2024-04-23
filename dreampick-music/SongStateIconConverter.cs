using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace dreampick_music;

public class SongStateIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        MediaState item = value is MediaState ? (MediaState)value : MediaState.Pause;

        return item == MediaState.Play ? ((App)Application.Current).Resources["ImgPause"] : ((App)Application.Current).Resources["ImgPlay"];
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}