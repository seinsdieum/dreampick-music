using System;
using System.Reflection.Metadata;
using System.Windows.Media.Imaging;

namespace dreampick_music.Models;

public abstract class Person : IUser
{
    public string ID { get; set; }
    public string Name { get; set; }
    public BitmapImage Image { get; set; }

    public int Subscribers { get; set; } = 0;
    public int Subscribes { get; set; } = 0;

    public abstract string Password
    {
        get;
        set;
    }
    
    
    
    
    
}