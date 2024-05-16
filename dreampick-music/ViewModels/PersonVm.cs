using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;
using Application = System.Windows.Forms.Application;

namespace dreampick_music;

public class PersonVm : INotifyPropertyChanged
{


    private string userId = "";
    private NotifyTaskCompletion<User> user;
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




    public NotifyTaskCompletion<User> User
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

        var userRepository = new UserRepository();
        var postsAsync = await userRepository.GetUserPosts(userId);

        return new ObservableCollection<PostVm>(
            postsAsync.Select(p => new PostVm() { Post = p} )
        );

    }

    private void LoadUserAsync()
    {
        
        var userRepository = new UserRepository();

        
        User = new NotifyTaskCompletion<User>(userRepository.GetById(userId));
        UserPosts = new NotifyTaskCompletion<ObservableCollection<PostVm>>(LoadPostList());
        SubscribersCount = new NotifyTaskCompletion<int>(userRepository.GetSubscribersCount(userId));
        SubscriptionsCount = new NotifyTaskCompletion<int>(userRepository.GetFollowersCount(userId));
        IsSubscribed = 
            new NotifyTaskCompletion<bool>(userRepository.GetIsFollowed(userId, AccountVm.Instance.AccountPerson.Id));
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
        var userRepository = new UserRepository();

        
        var a = userRepository.Follow(userId, AccountVm.Instance.AccountPerson.Id);
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