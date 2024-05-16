using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace dreampick_music.ValidationRules;

public class PasswordValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var regex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,16}$");
        return new ValidationResult(value is string RegPassword && regex.IsMatch(RegPassword), "Password:  min8, 1 letter u, 1 letter U, 1 number");
    }
}