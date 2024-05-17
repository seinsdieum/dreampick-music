using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using dreampick_music.DbRepositories;
using dreampick_music.Models;

namespace dreampick_music;

public class PlaylistPageVm : INotifyPropertyChanged
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


    private PlaylistRepository playlistRepo = new PlaylistRepository();

    private string albumid;
    private NotifyTaskCompletion<ObservableCollection<TrackListenVm>> tracks;
    private NotifyTaskCompletion<DbContexts.Playlist> album;
    private NotifyTaskCompletion<bool> isSubscribed;
    private NotifyTaskCompletion<int> relationsCount;

    public NotifyTaskCompletion<bool> IsSubscribed
    {
        get => isSubscribed;
        set
        {
            isSubscribed = value;
            OnPropertyChanged(nameof(IsSubscribed));
        }
    }

    public NotifyTaskCompletion<int> RelationsCount
    {
        get => relationsCount;
        set
        {
            relationsCount = value;
            OnPropertyChanged(nameof(RelationsCount));
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


    public NotifyTaskCompletion<DbContexts.Playlist> Album
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
        IsSubscribed = new NotifyTaskCompletion<bool>(playlistRepo.GetIsLiked(albumid, AccountVm.Instance.AccountPerson.Id));

        // TODO
        RelationsCount = new NotifyTaskCompletion<int>(playlistRepo.GetLikesCount(albumid));
        Album = new NotifyTaskCompletion<DbContexts.Playlist>(playlistRepo.GetById(albumid));

        var res = await Album.Task;
        

        return new ObservableCollection<TrackListenVm>(
            res.Tracks.Select(t => new TrackListenVm()
            {
                Track = t
            })
        );

    }

    private async Task<bool> SwitchUserRelation(bool isSubbed)
    {
        var repo = new PlaylistRepository();

        Task a;

        a = isSubbed ? repo.RemoveLike(albumid, AccountVm.Instance.AccountPerson.Id) : repo.AddLike(albumid, AccountVm.Instance.AccountPerson.Id);

        await a;

        if (a.IsFaulted) return isSubbed;
        
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