using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;

namespace dreampick_music;

public class AlbumPageVm : INotifyPropertyChanged
{
    
    #region VmContext
    
    
    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));

    private void DestroyObjects()
    {
        Album = null;
        albumid = null;
    }
    
    

    #endregion

    

    private string albumid;
    private NotifyTaskCompletion<ObservableCollection<TrackListenVm>> tracks;
    private NotifyTaskCompletion<Playlist> album;
    private NotifyTaskCompletion<bool> isSubscribed;

    public NotifyTaskCompletion<bool> IsSubscribed
    {
        get => isSubscribed;
        set
        {
            isSubscribed = value;
            OnPropertyChanged(nameof(IsSubscribed));
        }
    }


    public string AlbumId
    {
        set
        {
            albumid = value;
            LoadAlbum();
        }
    }
    
    public PlayerVm Player => PlayerVm.Instance;



    public NotifyTaskCompletion<ObservableCollection<TrackListenVm>> Tracks
    {
        get => tracks;
        set
        {
            tracks = value;
            OnPropertyChanged(nameof(Tracks));
        }
    }


    public NotifyTaskCompletion<Playlist> Album
    {
        get => album;
        set
        {
            album = value;
            OnPropertyChanged(nameof(Album));
        }
    }

    private async Task<ObservableCollection<TrackListenVm>> GetTracks()
    {
        Album = new NotifyTaskCompletion<Playlist>(PlaylistDAO.Instance.GetAsync(albumid));

        var res = await Album.Task;

        IsSubscribed = new NotifyTaskCompletion<bool>(PlaylistDAO.Instance.UserRelated(AccountVm.Instance.AccountPerson.Result.ID,
            res.ID));

        return new ObservableCollection<TrackListenVm>(
            res.Tracks.Select(t => new TrackListenVm()
            {
                Track = t
            })
        );

    }

    private async Task<bool> SwitchUserRelation(bool isSubbed)
    {
        if (Album.Result is null || AccountVm.Instance.AccountPerson is null) return false;

        await PlaylistDAO.Instance.RelateUser(AccountVm.Instance.AccountPerson.Result.ID, Album.Result.ID);
        return !isSubbed;
    }
    

    private void LoadAlbum()
    {
        Tracks = new NotifyTaskCompletion<ObservableCollection<TrackListenVm>>(GetTracks());
    }

    public ButtonCommand PlayAlbumCommand => new ButtonCommand((o =>
    {
        if (o is not string id) return;
        PlayerVm.Instance.PlayNewQueue(album.Result, id);
        OnPropertyChanged(nameof(Album));
    }));

    public ButtonCommand RelateCommand => new ButtonCommand(o =>
    {
        if (!isSubscribed.IsCompleted) return;
        var a = isSubscribed;
        IsSubscribed = new NotifyTaskCompletion<bool>(SwitchUserRelation(isSubscribed.Result));
    });


    # region PropertyEvents

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    # endregion
}