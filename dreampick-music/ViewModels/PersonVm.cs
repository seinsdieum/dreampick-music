using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;
using dreampick_music.Views;
using Application = System.Windows.Forms.Application;

namespace dreampick_music;

public class PersonVm : INotifyPropertyChanged
{

    private string userId = "";
    private NotifyTaskCompletion<Models.Person> user;
    private NotifyTaskCompletion<bool> isSubscribed;
    private NotifyTaskCompletion<int> subscribersCount;
    private NotifyTaskCompletion<int> subscriptionsCount;
    private NotifyTaskCompletion<ObservableCollection<PostVm>> userPosts;



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


    public NotifyTaskCompletion<bool> IsSubscribed
    {
        get => isSubscribed;
        set
        {
            isSubscribed = value;
            OnPropertyChanged(nameof(IsSubscribed));
        }
    }
    

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



    public NotifyTaskCompletion<ObservableCollection<PostVm>> UserPosts
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
    
    
    public ButtonCommand NavigateSubsctibersCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            NavigationVm.Instance.Navigate(new UserCollection(id, UserCollectionType.Subscribers));
        }
    });
    
    public ButtonCommand NavigateSubscriptionsCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            NavigationVm.Instance.Navigate(new UserCollection(id, UserCollectionType.Subscriptions));
        }
    });

    public ButtonCommand SubscribeCommand => new ButtonCommand(o =>
    {
        if (!IsSubscribed.IsSuccessfullyCompleted) return;
        var old = IsSubscribed.Result;
        IsSubscribed = new NotifyTaskCompletion<bool>(Subscribe(old));
    });
    
    public ButtonCommand NavigatePlaylistCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            NavigationVm.Instance.Navigate(new AlbumPage(id));
        }
    });
    
    public ButtonCommand NavigatePostLikesCommand => new ButtonCommand(o =>
    {
        if (o is not string id) return;
        NavigationVm.Instance.Navigate(new UserCollection(id, UserCollectionType.PostLikes));
    });
    
    

    private async Task<ObservableCollection<PostVm>> LoadPostList()
    {
        var postsAsync = await PostDAO.Instance.UserCollectionAsync(userId);

        return new ObservableCollection<PostVm>(
            postsAsync.Select(p => new PostVm() { Post = p} )
        );

    }

    private void LoadUserAsync()
    {
        User = new NotifyTaskCompletion<Models.Person>(UserDAO.Instance.GetAsync(userId));
        UserPosts = new NotifyTaskCompletion<ObservableCollection<PostVm>>(LoadPostList());
        SubscribersCount = new NotifyTaskCompletion<int>(UserDAO.Instance.SubscribersCountAsync(userId));
        SubscriptionsCount = new NotifyTaskCompletion<int>(UserDAO.Instance.SubscriptionsCountAsync(userId));
        IsSubscribed = 
            new NotifyTaskCompletion<bool>(UserDAO.Instance.IsSubscribedAsync
                    (AccountVm.Instance.AccountPerson.Result.ID, userId));
    } 
    
    private void DestroyObjects()
    {
        User = null;
        UserPosts = null;
        SubscriptionsCount = null;
        SubscribersCount = null;
        userId = null;
    }
    
    
    private async Task<bool> Subscribe(bool oldValue)
    {
        var a = UserDAO.Instance.SubscribeAsync(AccountVm.Instance.AccountPerson.Result.ID, userId);
        await a;
        if (a.IsCompletedSuccessfully) return !oldValue;
        
        throw new Exception();
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}