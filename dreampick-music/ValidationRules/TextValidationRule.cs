using System.Globalization;
using System.Windows.Controls;

namespace dreampick_music.ValidationRules;

public class TextValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        
        var val = value is string text && !string.IsNullOrEmpty(text);
        return new ValidationResult(val, "empty text");
    }
    
}