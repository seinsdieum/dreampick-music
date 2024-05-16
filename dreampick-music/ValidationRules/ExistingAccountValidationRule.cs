using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using dreampick_music.DbRepositories;

namespace dreampick_music.ValidationRules;

public class ExistingAccountValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var repo = new AccountRepository();
        var val = value is string str;

        if (value is not string v) return new ValidationResult(val, "username already exisfin");
        var a = repo.Exists(v);
        a.Wait();
        val = val && !a.Result;

        return new ValidationResult(val, "username already exisfin");
    }

}