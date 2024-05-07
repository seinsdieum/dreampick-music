using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using FontAwesome.WPF;

namespace dreampick_music.Models;

public class Track
{
    public string ID { get; set; }
    public Uri Source { get; set; }

    public string Name { get; set; }

    public double Duration
    {
        get;
        set;
    }

    public Playlist Album { get; set; }
    
    public DateTime ReleaseDate { get; set; }


    public string Lyrics
    {
        get;
        set;
    }
}