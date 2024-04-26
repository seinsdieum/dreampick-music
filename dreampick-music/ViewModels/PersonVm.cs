using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using dreampick_music.Models;
using Application = System.Windows.Forms.Application;

namespace dreampick_music;

public class PersonVm : INotifyPropertyChanged
{

    private string userId = "";

    public string UserId
    {
        get
        {
            return userId;
        }
        set
        {
            userId = value;
            LoadUserAsync();
        }
    }

    private NotifyTaskCompletion<int> subscribersCount;

    public NotifyTaskCompletion<int> SubscribersCount
    {
        get
        {
            return subscribersCount;
        }
        set
        {
            subscribersCount = value; OnPropertyChanged(nameof(SubscribersCount));
        }
    }
    
    
    
    private NotifyTaskCompletion<int> subscriptionsCount;

    public NotifyTaskCompletion<int> SubscriptionsCount
    {
        get
        {
            return subscriptionsCount;
        }
        set
        {
            subscriptionsCount = value; OnPropertyChanged(nameof(SubscriptionsCount));
        }
    }


    private NotifyTaskCompletion<ObservableCollection<Post>> userPosts;

    public NotifyTaskCompletion<ObservableCollection<Post>> UserPosts
    {
        get
        {
            return userPosts;
        }
        set
        {
            userPosts = value; OnPropertyChanged(nameof(UserPosts));
        }
    }


    private NotifyTaskCompletion<Models.Person> user;


    public NotifyTaskCompletion<Models.Person> User
    {
        get
        {
            return user;
        }
        set
        {
            user = value; OnPropertyChanged(nameof(User));
        }
    }

    public ButtonCommand BackCommand
    {
        get
        {
            return new ButtonCommand((o =>
            {
                if (NavigationVm.Instance is NavigationVm vm)
                {
                    vm.ClearNavigateBack(DestroyObjects);
                }
            }));
        }
    }
    
    

    private void DestroyObjects()
    {
        User = null;
        UserPosts = null;
        SubscriptionsCount = null;
        SubscribersCount = null;
        userId = null;
    }
    
    public ButtonCommand NavigateSubsctibersCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                if (o is string id && NavigationVm.Instance.Navigation is NavigationService service)
                {
                    service.Navigate(new Subscribers(id));
                }
            });
        }
    }
    
    public ButtonCommand NavigateSubscriptionsCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                if (o is string id && NavigationVm.Instance.Navigation is NavigationService service)
                {
                    service.Navigate(new Subsctiptions(id));
                }
            });
        }
    }


    private void LoadUserAsync()
    {
        User = new NotifyTaskCompletion<Models.Person>(PlatformDAO.Instance.LoadPersonAsync(userId));
        UserPosts = new NotifyTaskCompletion<ObservableCollection<Post>>(PlatformDAO.Instance.LoadUserPostsAsync(userId));
        SubscribersCount = new NotifyTaskCompletion<int>(PlatformDAO.Instance.LoadUserSubscribersAsync(userId));
        SubscriptionsCount = new NotifyTaskCompletion<int>(PlatformDAO.Instance.LoadUserSubscriptionsAsync(userId));
    } 
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}