using System;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows.Media.Imaging;

namespace dreampick_music.Models;

public class Playlist
{
    public PlaylistType Type;

    public string ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public BitmapImage Image { get; set; }

    public Person Author;

    public ObservableCollection<Track> Tracks { get; set; }

    public Playlist()
    {
        Tracks = new ObservableCollection<Track>();
        ID = new Random().Next(1000).ToString();
        Name = "NONAME";
    }


    public bool HasTrack(int index)
    {
        return index >= 0 && Tracks.Count > index;
    }
    
    
}