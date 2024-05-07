namespace dreampick_music.Models;

public class User : Person
{
    private string password;

    public override string Password
    {
        get { return "sdfs"; }
        set { password = value; }
    }
    

    public User()
    {
        base.Name = "undefined";
    }
}