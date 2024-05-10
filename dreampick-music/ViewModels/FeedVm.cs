using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using dreampick_music.DB;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class FeedVm : INotifyPropertyChanged
{
    private bool postsLoaded = false;

    private NotifyTaskCompletion<ObservableCollection<PostVm>> posts;
    

    public NotifyTaskCompletion<ObservableCollection<PostVm>> Posts
    {
        get
        {
            return posts;
        }
        set
        {
            posts = value;
            OnPropertyChanged(nameof(Posts));
        }
    }

    public ButtonCommand NavigateUserCommand => new ButtonCommand(o =>
    {
        if (o is string id && NavigationVm.Instance.Navigation is NavigationService service)
        {
            service.Navigate(new Person(id) {KeepAlive = false});
        }
    });

    public FeedVm()
    {
        LoadPostsAsync();
    }

    private async Task<ObservableCollection<PostVm>> LoadPostList()
    {
        var postsAsync = await PostDAO.Instance.CollectionAsync();

        return new ObservableCollection<PostVm>(
            postsAsync.Select(p => new PostVm() { Post = p} )
        );

    }


    private void LoadPostsAsync()
    {
        Posts = new NotifyTaskCompletion<ObservableCollection<PostVm>>(
            LoadPostList()
        );
    }
    
    
    public ButtonCommand NavigatePlaylistCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            NavigationVm.Instance.Navigate(new AlbumPage(id));
        }
    });
    
    public ButtonCommand RefreshCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                LoadPostsAsync();
            });
        }
    }

    public ButtonCommand NavigateToCreationCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.Navigate(new CreatePost());
    });

    public ButtonCommand NavigatePostLikesCommand => new ButtonCommand(o =>
    {
        if (o is not string id) return;
        NavigationVm.Instance.Navigate(new UserCollection(id, UserCollectionType.PostLikes));
    });
    
    
    


    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    
    

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}