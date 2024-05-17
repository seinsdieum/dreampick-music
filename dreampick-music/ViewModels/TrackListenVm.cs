using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using dreampick_music.DbRepositories;
using dreampick_music.Models;

namespace dreampick_music;

public class TrackListenVm : INotifyPropertyChanged
{

    private NotifyTaskCompletion<bool> _likeIsSet;
    private DbContexts.Track _track;

    private bool isPlaying;

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            OnPropertyChanged(nameof(IsPlaying));
        }
    }

    public DbContexts.Track Track
    {
        get => _track;
        set
        {
            _track = value;
            OnPropertyChanged(nameof(Track));
            LoadTrackInfo();
        }
    }
    
    public ButtonCommand NavigateArtistCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.Navigate(new Person(Track.Playlist.User.Id));
    });


    public NotifyTaskCompletion<bool> LikeIsSet
    {
        get => _likeIsSet;
        set
        {
            _likeIsSet = value;
            OnPropertyChanged(nameof(LikeIsSet));
        }
    }

    private async Task<bool> SetTrackLike(bool actual)
    {

        var trackRepository = new TrackRepository();

        Task a;
        a = !actual ? trackRepository.AddLike(Track.Id, AccountVm.Instance.AccountPerson.Id) : trackRepository.RemoveLike(Track.Id, AccountVm.Instance.AccountPerson.Id);
        await a;
        
        if (a.IsFaulted) return actual;
        return !actual;
    }

    private void OnTrackChange(object? sender, PropertyChangedEventArgs args)
    {
        if ( args.PropertyName != nameof(PlayerVm.Instance.CurrentTrack) )
            return;
        
        IsPlaying = PlayerVm.Instance.CurrentTrack.Id == Track.Id;
    }

    private void LoadTrackInfo()
    {
        
        var trackRepository = new TrackRepository();

        
        isPlaying = PlayerVm.Instance.CurrentTrack.Id == Track.Id;
        PlayerVm.Instance.PropertyChanged += OnTrackChange;
        LikeIsSet = new NotifyTaskCompletion<bool>(trackRepository.GetIsLiked(Track.Id, AccountVm.Instance.AccountPerson.Id));
    }

    ~TrackListenVm()
    {
        PlayerVm.Instance.PropertyChanged -= OnTrackChange;
    }


    public ButtonCommand SetLike => new ButtonCommand(o =>
    {
        if (!LikeIsSet.IsCompleted) return;
        
        var actual = LikeIsSet.Result;
        LikeIsSet = new NotifyTaskCompletion<bool>(SetTrackLike(actual));
    });
    

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
