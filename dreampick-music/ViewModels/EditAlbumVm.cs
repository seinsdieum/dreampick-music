using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using Microsoft.IdentityModel.Tokens;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace dreampick_music;

public class EditAlbumVm : HistoryVm, INotifyDataErrorInfo
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
        set
        {
            Set(ref albumName, value);
            ValidateAlbum();
        }
    }
    
    [UndoRedo]
    public string AlbumDescription
    {
        get => albumDescription;
        set
        {
            Set(ref albumDescription, value);
            ValidateAlbum();

        }
    }

    [UndoRedo]
    public BitmapImage ImageSource
    {
        get => imageSource;
        set
        {
            Set(ref imageSource, value);
            ValidateAlbum();
        } 
    }

    [UndoRedo]
    public ObservableCollection<TrackVm> Tracks
    {
        get => tracks;
        set
        {
            Set(ref tracks, value);
            ValidateAlbum();
        }
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
    
    public ButtonCommand AddTrackCommand => new ButtonCommand((o =>
    {
        var tracks = Tracks;
        tracks.Add(new TrackVm(AccountVm.Instance.AccountPerson.Id, Utils.GenerateRandomString(10)));
        Set(ref this.tracks, tracks);
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
    }), o => !HasErrors && errorsChecked);
    
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
                Playlist = tracksPlaylist,
                Id = track.TrackId,
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
                Playlist = tracksPlaylist,
                Id = track.TrackId,
            }).ToList();

        hasProblems = (t.Count < tracks.Count);
        
        
        return t;
    }

    private Playlist CreatePlaylist()
    {
        var playlist = new Playlist()
        {
            Name = AlbumName,
            Image = ImageSource is null ? null : Utils.GetByteArrayFromImage(ImageSource),
            User = loadedPlaylist.Result.User,
            Description = AlbumDescription
        };
        playlist.Tracks = new List<Track>(VmToTracks(playlist));
        return playlist;
    }
    
    private Playlist CreatePlaylist(out bool hasProblems)
    {
        ValidateAlbum();

        if (HasErrors)
        {
            hasProblems = true;
            return null;
        }
        
        var playlist = new Playlist()
        {
            Id = loadedPlaylist.Result.Id,
            Name = AlbumName,
            Image = ImageSource is null ? null : Utils.GetByteArrayFromImage(ImageSource),
            Description = AlbumDescription,
            
            User = loadedPlaylist.Result.User
        };
        playlist.Tracks = new List<Track>(VmToTracks(playlist, out hasProblems));
        return playlist;
    }
    

    private void PublishPlaylist()
    {

        var playlistRepository = new PlaylistRepository();
        
        var playlist = CreatePlaylist(out var hasProblems);
        
        
        if(hasProblems || loadedPlaylist.IsNotCompleted) return;
        
        _ = playlistRepository.Update(playlist);
    }

    private async Task LoadAlbum()
    {
        
        var playlistRepository = new PlaylistRepository();

        
        LoadedPlaylist = new NotifyTaskCompletion<Playlist>(playlistRepository.GetWithTracksById(albumId));
        await LoadedPlaylist.Task;

        if (LoadedPlaylist.IsSuccessfullyCompleted)
        {
            AlbumName = loadedPlaylist.Result.Name;
            AlbumDescription = loadedPlaylist.Result.Description;
            Tracks = new ObservableCollection<TrackVm>(
                loadedPlaylist.Result.Tracks.Select(track => new TrackVm(loadedPlaylist.Result.User.Id, track.Id) { Source = track.Source, Name = track.Name, TrackId = track.Id}).ToList());

            ImageSource = Utils.GetBitmapImage(loadedPlaylist.Result.Image);
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

    #region AlbumErrors

    private bool errorsChecked = false;

    public ButtonCommand RecheckValidation => new ButtonCommand(o =>
    {
        ValidateAlbum();
    });

    public List<string> AlbumErrors => AlbumValidationErrors.SelectMany(item => item.Value).ToList();

    private void ValidateAlbum()
    {
        
        ClearErrors(nameof(AlbumName));
        ClearErrors(nameof(AlbumDescription));
        ClearErrors(nameof(ImageSource));
        ClearErrors(nameof(Tracks));
        if(string.IsNullOrEmpty(albumName)) AddError(nameof(AlbumName), "album name is empty");
        if(string.IsNullOrEmpty(albumDescription)) AddError(nameof(AlbumDescription), "album description is empty");
        if(imageSource is not BitmapImage ) AddError(nameof(ImageSource), "album image not loaded");
        
        if(tracks.IsNullOrEmpty()) AddError(nameof(Tracks), "tracklist is empty");
        else
        {
            for (var i = 0; i < tracks.Count; i++)
            {
                if (string.IsNullOrEmpty(tracks[i].Name))
                {
                    AddError(nameof(Tracks), "track " + (i+1) + " has no name");
                    continue;
                }

                if (!tracks[i].SourceIsLoaded)
                {
                    AddError(nameof(Tracks), "track " + $"\"{tracks[i].Name}\"" + " has no source");
                }
            }
        }

        errorsChecked = true;
        
        NotifyPropertyChanged(nameof(AlbumErrors));

    }
    
    
    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
    
    private Dictionary<string, List<string>> AlbumValidationErrors = new Dictionary<string, List<string>>();
    private void AddError(string propertyName, string error)
    {
        if (!AlbumValidationErrors.ContainsKey(propertyName))
            AlbumValidationErrors[propertyName] = new List<string>();

        if (!AlbumValidationErrors[propertyName].Contains(error))
        {
            AlbumValidationErrors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    private void ClearErrors(string propertyName)
    {
        if (AlbumValidationErrors.ContainsKey(propertyName))
        {
            AlbumValidationErrors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }
    

    public IEnumerable GetErrors(string? propertyName)
    {
        throw new NotImplementedException();
    }

    public bool HasErrors => AlbumValidationErrors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

    #endregion
    
}
