using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace dreampick_music.Models;

public class AudioPlayerModel
{
    
    
    
    public static AudioPlayerModel Instance = new AudioPlayerModel();

    /*public Playlist GeneratePlaylist(string id,string name, ObservableCollection<Track> tracks, Person author,string description = "", BitmapImage image = null)
    {
        var playlist = new Playlist()
        {
            Name = name,
            Tracks = tracks,
            Description = description,
            ID = id,
            Author = author,
            Image = image
        };

        return playlist;
    }*/

    public int GetQueueIndex(DbContexts.Playlist playlist, string trackID)
    {
        var index = playlist.Tracks.IndexOf(playlist.Tracks.Single(track => track.Id == trackID));

        return index == -1 ? 0 : index;
    }

    /*public Playlist RandomizePlaylist(Playlist p)
    {
        var rand = new Random();
        for (int i = 0; i < p.Tracks.Count; i++)
        {
            var a = rand.Next(0, p.Tracks.Count);
            while ((a = rand.Next(0, p.Tracks.Count)) == i)
            {
            }

            (p.Tracks[i], p.Tracks[a]) = (p.Tracks[a], p.Tracks[i]);

        }

        return new Playlist()
        {
            Name = p.Name,
            Tracks = p.Tracks,
            Author = p.Author,
            ID = p.ID,
            Image = p.Image,
            Description = p.Description,
            Type = p.Type,
            
        };
    }*/
    
}