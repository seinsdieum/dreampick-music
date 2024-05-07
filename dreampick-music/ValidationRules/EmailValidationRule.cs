using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace dreampick_music.ValidationRules;

public class EmailValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var regex = new Regex("^\\S+@\\S+\\.\\S+$");
        var result = value is string email && regex.IsMatch(email);
        return new ValidationResult(result, "email not valid");
    }
}