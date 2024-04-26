using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using dreampick_music.Models;

namespace dreampick_music;

public class FeedVm : INotifyPropertyChanged
{
    private bool postsLoaded = false;

    private NotifyTaskCompletion<ObservableCollection<Post>> posts;

    public NotifyTaskCompletion<ObservableCollection<Post>> Posts
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


    private void LoadPostsAsync()
    {
        Posts = new NotifyTaskCompletion<ObservableCollection<Post>>(
            PlatformDAO.Instance.LoadPostsAsync()
        );
    }
    
    
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
    
    


    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    
    

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}