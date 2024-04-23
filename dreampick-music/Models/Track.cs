using System;

namespace dreampick_music.Models;

public class Track
{
    public Uri Source
    {
        get;
        set;
    }
    
    public string Name
    {
        get;
        set;
    }

    public Artist Artist
    {
        get;
        set;
    }

    public Playlist Album
    {
        get;
        set;
    }

    public Track()
    {
        Artist = new Artist();
        Artist.Name = "undefined";
    }
}