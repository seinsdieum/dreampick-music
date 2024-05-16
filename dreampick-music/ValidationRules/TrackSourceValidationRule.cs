using System.Globalization;
using System.Windows.Controls;
using dreampick_music.DbContexts;

namespace dreampick_music.ValidationRules;

public class TrackSourceValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        
        var val = value is Track track && !(string.IsNullOrEmpty(track.Source.AbsoluteUri));
        return new ValidationResult
            (val, ("track "+(value is Track t ? t.Name : "undefined" + " - " + "source missing"  )) );
    }
}