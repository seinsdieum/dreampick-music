using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using dreampick_music.DB;
using dreampick_music.Models;

namespace dreampick_music;

public class TrackListenVm : INotifyPropertyChanged
{
    
    private NotifyTaskCompletion<bool> _likeIsSet;
    private Track _track;

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

    public Track Track
    {
        get => _track;
        set
        {
            _track = value;
            OnPropertyChanged(nameof(Track));
            LoadTrackInfo();
        }
    }

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
        var a = await TrackDAO.Instance.RelateAsync(AccountVm.Instance.AccountPerson.Result.ID, Track.ID);

        return !actual;
    }

    private void OnTrackChange(object? sender, PropertyChangedEventArgs args)
    {
        if ( args.PropertyName != nameof(PlayerVm.Instance.CurrentTrack) )
            return;
        
        IsPlaying = PlayerVm.Instance.CurrentTrack.ID == Track.ID;
    }

    private void LoadTrackInfo()
    {
        isPlaying = PlayerVm.Instance.CurrentTrack.ID == Track.ID;
        PlayerVm.Instance.PropertyChanged += OnTrackChange;
        LikeIsSet = new NotifyTaskCompletion<bool>(TrackDAO.Instance.IsRelatedAsync(AccountVm.Instance.AccountPerson.Result.ID, Track.ID));
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
