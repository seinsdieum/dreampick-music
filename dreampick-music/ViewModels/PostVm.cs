using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music;

public class PostVm : INotifyPropertyChanged
{

    private NotifyTaskCompletion<bool> _likeIsSet;

    private DbContexts.Post _post;

    public bool HasPlaylist
    {
        get => Post.Playlist is DbContexts.Playlist;
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

    public DbContexts.Post Post
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
        var postRepository = new PostRepository();

        var a = false;

        if (prevState)
        {
           a = await postRepository.RemoveLike(Post.Id, AccountVm.Instance.AccountPerson.Id);
        }
        else
        {
          a = await postRepository.AddLike(Post.Id, AccountVm.Instance.AccountPerson.Id);
        }

        if (!a) return prevState;

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
        var postRepository = new PostRepository();
        LikeIsSet = new NotifyTaskCompletion<bool>(postRepository.GetIsLiked(Post.Id, AccountVm.Instance.AccountPerson.Id));
        OnPropertyChanged(nameof(HasPlaylist));
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}