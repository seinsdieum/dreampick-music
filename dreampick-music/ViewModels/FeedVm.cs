using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class FeedVm : INotifyPropertyChanged
{


    private string searchString = "";

    private bool searchWithPlaylist = false;

    public bool SearchWithPlaylist
    {
        get => searchWithPlaylist;
        set
        {
            searchWithPlaylist = value;
            OnPropertyChanged(nameof(SearchWithPlaylist));
            SearchPosts();
        }
    }

    public string SearchString
    {
        get => searchString;
        set
        {
            searchString = value;
            OnPropertyChanged(nameof(SearchString));
            SearchPosts();
        }
    }
    
    

    private Func<PostVm, bool> TextSearchCriteria
        =>
            vm => string.IsNullOrEmpty(searchString) || vm.Post.Text.Contains(searchString) || vm.Post.User.Username.Contains(searchString) ||
                  (vm.Post.Playlist != null && vm.Post.Playlist.Name.Contains(searchString));

    private Func<PostVm, bool> OnlyPlaylistSearchCriteria
        =>
            vm => vm.Post.Playlist != null;
    
    


    private NotifyTaskCompletion<ObservableCollection<PostVm>> posts;


    private ObservableCollection<PostVm> visiblePosts = new ObservableCollection<PostVm>();

    public ObservableCollection<PostVm> VisiblePosts
    {
        get => visiblePosts;
        set
        {
            visiblePosts = value;
            OnPropertyChanged(nameof(VisiblePosts));
        }
    }
    
    
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
        var postRepository = new PostRepository();
        var postsAsync = await postRepository.GetAll();
        
        var p =  new ObservableCollection<PostVm>(
            postsAsync.Select(p => new PostVm() { Post = p} )
        );

        VisiblePosts = p;

        return new ObservableCollection<PostVm>(
            postsAsync.Select(p => new PostVm() { Post = p} )
        );

    }

    private ObservableCollection<PostVm> SearchPosts(List<Func<PostVm, bool>> postCriterias)
    {
        if (Posts is null || !Posts.IsCompleted) return new ObservableCollection<PostVm>();
        
        return new ObservableCollection<PostVm>(Posts.Result.Where(x => postCriterias.All(p => p.Invoke(x))));
    }


    private void SearchPosts()
    {
        var list = new List<Func<PostVm, bool>>();
        
        if(searchWithPlaylist) list.Add(OnlyPlaylistSearchCriteria);
        list.Add(TextSearchCriteria);

        VisiblePosts = SearchPosts(list);
    }

    public ButtonCommand SearchCommand => new ButtonCommand(o =>
    {
        SearchPosts();
    });


    private void LoadPostsAsync()
    {
        Posts = new NotifyTaskCompletion<ObservableCollection<PostVm>>(
            LoadPostList()
        );
    }
    
    
    public ButtonCommand NavigatePlaylistCommand => new ButtonCommand(o =>
    {
        if (o is Playlist p)
        {
            
            NavigationVm.Instance.Navigate(new AlbumPage(p.Id, p.IsUserPlaylist));
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