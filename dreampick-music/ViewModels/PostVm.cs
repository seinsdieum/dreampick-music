using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using dreampick_music.DB;
using dreampick_music.Models;

namespace dreampick_music;

public class PostVm : INotifyPropertyChanged
{

    private NotifyTaskCompletion<bool> _likeIsSet;

    private Post _post;

    public bool HasPlaylist
    {
        get => Post.PostPlaylist is Playlist;
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

    public Post Post
    {
        get => _post;
        set
        {
            _post = value;
            OnPropertyChanged(nameof(Post));
            LoadPostInfo();
        }
    }

    private async Task<bool> SetLikeAsync(bool prevState)
    {
        var a = await PostDAO.Instance.RelateAsync(AccountVm.Instance.AccountPerson.Result.ID, Post.ID);

        if (!a) return false;

        if (prevState)
        {
            Post.Likes--;
            OnPropertyChanged(nameof(Post));
        }
        else
        {
            Post.Likes++;
            OnPropertyChanged(nameof(Post));
        }

        return !prevState;

    }

    public ButtonCommand SetLike => new ButtonCommand((o =>
    {
        if (LikeIsSet.IsCompleted)
        {
            var a = _likeIsSet.Result;
            LikeIsSet = new NotifyTaskCompletion<bool>(SetLikeAsync(a));
        }
    }));

    private void LoadPostInfo()
    {
        LikeIsSet = new NotifyTaskCompletion<bool>(
            PostDAO.Instance.IsRelatedAsync(AccountVm.Instance.AccountPerson.Result.ID, Post.ID));
        OnPropertyChanged(nameof(HasPlaylist));
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}