using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace dreampick_music.Models;

public class AudioPlayerModel
{
    
    
    
    public static AudioPlayerModel Instance = new AudioPlayerModel();

    public Playlist GeneratePlaylist(string id,string name, ObservableCollection<Track> tracks, Person author,string description = "", BitmapImage image = null)
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
    }

    public int GetQueueIndex(Playlist playlist, string trackID)
    {
        var index = playlist.Tracks.IndexOf(playlist.Tracks.Single(track => track.ID == trackID));

        return index == -1 ? 0 : index;
    }

    public PlayingQueueVm GenerateQueue(Playlist playlist, int index)
    {
        var queue = new PlayingQueueVm()
        {
            Queue = playlist,
            TrackIndex = index,
        };
        
        return queue;
    }
    
}