using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace dreampick_music;

public class PublishAudioVm : HistoryVm, INotifyDataErrorInfo
{


    private string albumName;

    private string albumDescription;
    private ObservableCollection<TrackVm> tracks = new ObservableCollection<TrackVm>();

    private Uri imageSource;

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
    public Uri ImageSource
    {
        get => imageSource;
        set
        { Set(ref imageSource, value);
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
        dialog.Filter = "Image files |*.jpg;*.png";
        var result = dialog.ShowDialog();
        if (result != true) return;

        string path = dialog.FileName;
        ImageSource = new Uri(path, UriKind.Absolute);
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
    
    public ButtonCommand RemoveTrackCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            Tracks.Remove(Tracks.Single(track => track.TrackId == id));
        }
    });
    
    public ButtonCommand PublishCommand => new ButtonCommand((o =>
    {
        PublishPlaylist();
    }), o => !HasErrors && errorsChecked);

    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));

    private void DestroyObjects()
    {
        // TODO implement
    }


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
                CreatedOn = DateTime.Now
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
                CreatedOn = DateTime.Now
            }).ToList();

        hasProblems = (t.Count < tracks.Count);
        
        
        return t;
    }

    private Playlist CreatePlaylist()
    {
        var playlist = new Playlist()
        {
            Name = AlbumName,
            Image = ImageSource is null ? null : Utils.GetByteArrayFromImage(new BitmapImage(ImageSource)),
            User = AccountVm.Instance.AccountPerson,
            Description = AlbumDescription,
            CreatedOn = DateTime.Now,
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
            Id = Utils.GenerateRandomString(10),
            Name = AlbumName,
            Image = ImageSource is null ? null : Utils.GetByteArrayFromImage(new BitmapImage(ImageSource)),
            UserId = AccountVm.Instance.AccountPerson.Id,
            Description = AlbumDescription,
            CreatedOn = DateTime.Now,
        };
        playlist.Tracks = new List<Track>(VmToTracks(playlist, out hasProblems));
        return playlist;
    }
    

    private void PublishPlaylist()
    {
        var playlistRepository = new PlaylistRepository();
        
        var playlist = CreatePlaylist(out var hasProblems);
        
        if(hasProblems) return;
        
        playlistRepository.Add(playlist);
        
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);

    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
    
    
    #region AlbumErrors


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
        if(imageSource is not Uri ) AddError(nameof(ImageSource), "album image not loaded");
        
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

    private bool errorsChecked = false;
    

    public IEnumerable GetErrors(string? propertyName)
    {
        throw new NotImplementedException();
    }

    public bool HasErrors => AlbumValidationErrors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

    #endregion
    
}