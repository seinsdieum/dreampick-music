using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace dreampick_music;

public class PlaylistEditPageVm : HistoryVm, INotifyDataErrorInfo
{

    private string referenceId;
    
        private string albumName;

    private string albumDescription;
    private ObservableCollection<Track> tracks = new ObservableCollection<Track>();

    private BitmapImage imageSource;

    public string ReferenceId
    {
        get => referenceId;
        set
        {
            referenceId = value;
            OnPropertyChanged(nameof(ReferenceId));
            LoadPlaylist();
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
    public ObservableCollection<Track> Tracks
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
        ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
    }));

    private async void AddTrack(string id)
    {
        var repo = new TrackRepository();
        var tracks = Tracks;

        var t = await repo.GetById(id);
        tracks.Add(t);
        
        Set(ref this.tracks, tracks);
    } 

    public ButtonCommand AddTrackCommand => new ButtonCommand((o =>
    {
        
        WindowModel.OpenRelatedTracksSelectionDialog(new TrackCollectionPage(TrackCollectionType.Liked, "", (o1 =>
        {
            if (o1 is not string id) return;
            AddTrack(id);
        })));
    }));
    
    
    public ButtonCommand RemoveTrackCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            var tracks = Tracks;
            tracks.Remove(Tracks.Single(track => track.Id == id));
            Set(ref this.tracks, tracks);
        }
    });

    private void UpdatePlaylist()
    {
        var repo = new PlaylistRepository();
        
        var playlist = CreatePlaylist();
        
        if(Tracks.Count <= 0) return;
        
        repo.UpdateCustom(playlist, Tracks.Select(t => t.Id).ToList());
        
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
        
    }
    
    public ButtonCommand PublishCommand => new ButtonCommand((o =>
    {
        if(string.IsNullOrEmpty(referenceId)) PublishPlaylist();
        else UpdatePlaylist();
    }), o => !HasErrors && errorsChecked);

    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));

    private void DestroyObjects()
    {
        // TODO implement
    }
    

    private Playlist CreatePlaylist()
    {
        var playlist = new Playlist()
        {
            Id = string.IsNullOrEmpty(referenceId) ? Utils.GenerateRandomString(10) : referenceId,
            Name = AlbumName,
            Image = ImageSource is null ? null : Utils.GetByteArrayFromImage(ImageSource),
            Description = AlbumDescription,
            CreatedOn = DateTime.Now,
            IsUserPlaylist = true,
            UserAddedTracks = new List<Track>()
        };
        return playlist;
    }
    

    private void PublishPlaylist()
    {
        var playlistRepository = new PlaylistRepository();
        
        var playlist = CreatePlaylist();
        
        if(Tracks.Count <= 0) return;
        
        
        playlistRepository.AddCustom(playlist, Tracks.Select(t => t.Id).ToList(), AccountVm.Instance.AccountPerson.Id);
        
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);

    }

    private async void LoadPlaylist()
    {
        if (string.IsNullOrEmpty(referenceId)) return;

        var repo = new PlaylistRepository();

        var a = await repo.GetCustomById(referenceId);

        Tracks = new ObservableCollection<Track>(a.UserAddedTracks);
        AlbumName = a.Name;
        AlbumDescription = a.Description;
        if(a.Image != null) ImageSource = Utils.GetBitmapImage(a.Image);

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
        if(string.IsNullOrEmpty(albumName)) AddError(nameof(AlbumName), Utils.GetLocalizedName("LNameEmpty"));
        if(string.IsNullOrEmpty(albumDescription)) AddError(nameof(AlbumDescription), Utils.GetLocalizedName("LDescriptionEmpty"));
        if(imageSource is not BitmapImage ) AddError(nameof(ImageSource), Utils.GetLocalizedName("LImageNotLoaded"));
        
        if(tracks.IsNullOrEmpty()) AddError(nameof(Tracks), Utils.GetLocalizedName("LTracklistEmpty"));

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