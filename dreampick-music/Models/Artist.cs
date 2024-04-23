using System.Reflection.Metadata;

namespace dreampick_music.Models;

public class Artist : Person
{
    private string password;

    public override string Password
    {
        get { return "sdfs"; }
        set { password = value; }
    }

    public Blob HeaderImage { get; set; }

    public Artist()
    {
        base.Name = "undefined";

    }
}