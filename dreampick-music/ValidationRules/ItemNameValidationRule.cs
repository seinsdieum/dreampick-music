using System.Globalization;
using System.Windows.Controls;
using dreampick_music.DbContexts;

namespace dreampick_music.ValidationRules;

public class ItemNameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        
        var val = value is string str && !(str.Length > 3);
        return new ValidationResult(val, ("dont leave fields empty") );
    }

}