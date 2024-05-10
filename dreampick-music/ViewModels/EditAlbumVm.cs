using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using dreampick_music.DB;
using dreampick_music.Models;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace dreampick_music;

public class EditAlbumVm : HistoryVm
{
    
    private string albumName;
    private string albumDescription;

    

    private ObservableCollection<TrackVm> tracks = new ObservableCollection<TrackVm>();

    
    private BitmapImage imageSource;
    
    private string albumId;
    private NotifyTaskCompletion<Playlist> loadedPlaylist;

    




    public string AlbumId
    {
        get => albumId;
        set
        {
            albumId = value;
            LoadAlbum();
        }
    }

    
    public NotifyTaskCompletion<Playlist> LoadedPlaylist
    {
        get
        {
            return loadedPlaylist;
        }
        set
        {
            loadedPlaylist = value;
            NotifyPropertyChanged();
        }
    }
    
    
    [UndoRedo]

    public string AlbumName
    {
        get => albumName;
        set => Set(ref albumName, value);
    }
    
    [UndoRedo]
    public string AlbumDescription
    {
        get => albumDescription;
        set => Set(ref albumDescription, value);
    }

    [UndoRedo]
    public BitmapImage ImageSource
    {
        get => imageSource;
        set => Set(ref imageSource, value);
    }

    [UndoRedo]
    public ObservableCollection<TrackVm> Tracks
    {
        get => tracks;
        set => Set(ref tracks, value);
    }
    

    public ButtonCommand LoadImageCommand => new ButtonCommand((o =>
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".jpg";
        dialog.Filter = "Image files (.jpg), (.png)|*.jpg;*.png";
        var result = dialog.ShowDialog();
        if (result != true) return;

        string path = dialog.FileName;
        ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
    }));

    public ButtonCommand PlayQueueCommand => new ButtonCommand((o =>
    {
        if (o is not string id) return;
        var tracksPlaylist = CreatePlaylist();
        PlayerVm.Instance.PlayNewQueue(tracksPlaylist, id);
    }));
    
    public ButtonCommand PublishCommand => new ButtonCommand((o =>
    {
        PublishPlaylist();
    }));
    
    public ButtonCommand RemoveTrackCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            Tracks.Remove(Tracks.Single(track => track.TrackId == id));
        }
    });


    private List<Track> VmToTracks(Playlist tracksPlaylist)
    {
        return tracks
            .Where(track => track.SourceIsLoaded && !string.IsNullOrEmpty(track.Name))
            .Select(track => new Track()
            {
                Name = track.Name,
                Source = track.Source,
                Album = tracksPlaylist,
                ID = track.TrackId,
            }).ToList();
    }
    
    private List<Track> VmToTracks(Playlist tracksPlaylist, out bool hasProblems)
    {
        var t = tracks
            .Where(track => track.SourceIsLoaded && !string.IsNullOrEmpty(track.Name))
            .Select(track => new Track()
            {
                Name = track.Name,
                Source = track.Source,
                Album = tracksPlaylist,
                ID = track.TrackId,
            }).ToList();

        hasProblems = (t.Count < tracks.Count);
        
        
        return t;
    }

    private Playlist CreatePlaylist()
    {
        var playlist = new Playlist()
        {
            Name = AlbumName,
            Type = PlaylistType.ALBUM,
            Image = ImageSource is null ? null : ImageSource,
            Author = loadedPlaylist.Result.Author
        };
        playlist.Tracks = new ObservableCollection<Track>(VmToTracks(playlist));
        return playlist;
    }
    
    private Playlist CreatePlaylist(out bool hasProblems)
    {
        var playlist = new Playlist()
        {
            ID = loadedPlaylist.Result.ID,
            Name = AlbumName,
            Type = PlaylistType.ALBUM,
            Image = ImageSource is null ? null : ImageSource,

            Author = loadedPlaylist.Result.Author
        };
        playlist.Tracks = new ObservableCollection<Track>(VmToTracks(playlist, out hasProblems));
        return playlist;
    }
    

    private void PublishPlaylist()
    {
        var playlist = CreatePlaylist(out var hasProblems);
        
        if(hasProblems || loadedPlaylist.IsNotCompleted) return;
        
        _ = PlaylistDAO.Instance.UpdateAsync(playlist, loadedPlaylist.Result.Tracks);
    }

    private async Task LoadAlbum()
    {
        LoadedPlaylist = new NotifyTaskCompletion<Playlist>(PlaylistDAO.Instance.GetAsync(albumId));
        await LoadedPlaylist.Task;

        if (LoadedPlaylist.IsSuccessfullyCompleted)
        {
            AlbumName = loadedPlaylist.Result.Name;
            AlbumDescription = loadedPlaylist.Result.Description;
            Tracks = new ObservableCollection<TrackVm>(
                loadedPlaylist.Result.Tracks.Select(track => new TrackVm(loadedPlaylist.Result.Author.ID, track.ID) { Source = track.Source, Name = track.Name, TrackId = track.ID}).ToList());

            ImageSource = loadedPlaylist.Result.Image;
        }
    }

    private void DestroyObjects()
    {
        
    }
    
    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));



    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}