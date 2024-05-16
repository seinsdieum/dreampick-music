using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using dreampick_music.DbContexts;

namespace dreampick_music.ValidationRules;

public partial class TrackNameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        
        var val = value is Track track && !(track.Name.Length > 0);
        return new ValidationResult(val, ("empty track name") );
    }

}