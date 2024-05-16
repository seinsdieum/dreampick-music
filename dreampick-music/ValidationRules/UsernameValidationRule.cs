using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using dreampick_music.DbRepositories;

namespace dreampick_music.ValidationRules;

public partial class UsernameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        
        var regex = MyRegex();
        var val = value is string RegName && !(RegName.Length > 3 && regex.IsMatch(RegName));
        return new ValidationResult(val, "username not valid");
    }

    [GeneratedRegex("^[a-zA-Z_](?!.*?\\.{2})[\\w.]{1,28}[\\w]$")]
    private static partial Regex MyRegex();
}