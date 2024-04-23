using System.Reflection.Metadata;
using System.Windows.Media.Imaging;

namespace dreampick_music.Models;

public class Artist : Person
{
    private string password;

    public override string Password
    {
        get { return "sdfs"; }
        set { password = value; }
    }

    public BitmapImage HeaderImage { get; set; }

    public Artist()
    {
        base.Name = "undefined";

    }
}