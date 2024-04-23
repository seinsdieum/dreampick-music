using System.Reflection.Metadata;

namespace dreampick_music.Models;

public abstract class Person : IUser
{
    public string ID { get; set; }
    public string Name { get; set; }
    public Blob Image { get; set; }

    public abstract string Password
    {
        get;
        set;
    }
    
    
    
    
    
}