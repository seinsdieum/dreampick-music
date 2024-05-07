using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;

namespace dreampick_music.Models;

public class AccountModel
{
    public static AccountModel Instance;
    public string ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}