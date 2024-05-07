using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FontAwesome.WPF;

namespace dreampick_music.Converters;

public class DateTimeToStampStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // here we are going to subtract the passed in DateTime from the current time converted to UTC
        TimeSpan ts = DateTime.Now.ToUniversalTime().Subtract((DateTime)value);
        int intDays = ts.Days;
        int intHours = ts.Hours;
        int intMinutes = ts.Minutes;
        int intSeconds = ts.Seconds;

        if (intDays > 0)
            return string.Format("{0} days ago", intDays);

        if (intHours > 0)
            return string.Format("{0} hours ago", intHours);

        if (intMinutes > 0)
            return string.Format("{0} minutes ago", intMinutes);

        if (intSeconds > 0)
            return string.Format("{0} seconds ago", intSeconds);

        // let's handle future times..just in case
        if (intDays < 0)
            return string.Format("in {0} days", Math.Abs(intDays));

        if (intHours < 0)
            return string.Format("in {0} hours", Math.Abs(intHours));

        if (intMinutes < 0)
            return string.Format("in {0} minutes", Math.Abs(intMinutes));

        if (intSeconds < 0)
            return string.Format("in {0} seconds", Math.Abs(intSeconds));

        return "a bit";
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}