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
            return $"{intDays} " + Utils.GetLocalizedName("LDays") + " " + Utils.GetLocalizedName("LAgo") ;

        if (intHours > 0)
            return $"{intDays} " + Utils.GetLocalizedName("LHours") + " " + Utils.GetLocalizedName("LAgo") ;


        if (intMinutes > 0)
            return $"{intDays} " + Utils.GetLocalizedName("LMinutes") + " " + Utils.GetLocalizedName("LAgo") ;


        if (intSeconds > 0)
            return Utils.GetLocalizedName("LRecently")  ;


        // let's handle future times..just in case
        if (intDays < 0)
            return Utils.GetLocalizedName("LRecently")  ;

        if (intHours < 0)
            return Utils.GetLocalizedName("LRecently")  ;

        if (intMinutes < 0)
            return Utils.GetLocalizedName("LRecently")  ;

        if (intSeconds < 0)
            return Utils.GetLocalizedName("LRecently")  ;

        return Utils.GetLocalizedName("LRecently")  ;
    }
     
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }  
}