using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;
using dreampick_music.Views;
using Microsoft.Win32;

namespace dreampick_music;

public class AccountPageVm : INotifyPropertyChanged
{
    public AccountVm Account => AccountVm.Instance;
    
    
    private string  userId = "";

    private BitmapImage changeAvatar;
    private string changeName = "";
    private string changeEmail = "";
    private string changePassword = "";
    private string verifyPassword = "";

    private bool changePasswordVisible = false;
    private bool verifyPasswordVisible = false;


    public BitmapImage ChangeAvatar
    {
        get => changeAvatar;
        set
        {
            changeAvatar = value;
            OnPropertyChanged(nameof(ChangeAvatar));
        }
    }
    public string ChangeName
    {
        get => changeName;
        set
        {
            changeName = value;
            OnPropertyChanged(nameof(ChangeName));
        }
    }
    
    public string ChangeEmail
    {
        get => changeEmail;
        set
        {
            changeEmail = value;
            OnPropertyChanged(nameof(ChangeEmail));
        }
    }
    
    public string ChangePassword
    {
        get => changePassword;
        set
        {
            changePassword = value;
            OnPropertyChanged(nameof(ChangePassword));
        }
    }
    
    public string VerifyPassword
    {
        get => verifyPassword;
        set
        {
            verifyPassword = value;
            OnPropertyChanged(nameof(VerifyPassword));
        }
    }

    public bool ChangePasswordVisible
    {
        get => changePasswordVisible;
        set
        {
            changePasswordVisible = value;
            OnPropertyChanged(nameof(ChangePasswordVisible));
        }
    }
    
    public bool VerifyPasswordVisible
    {
        get => verifyPasswordVisible;
        set
        {
            verifyPasswordVisible = value;
            OnPropertyChanged(nameof(VerifyPasswordVisible));
        }
    }

    
    
    public string UserId
    {
        get
        {
            return userId;
        }
        set
        {
            userId = value;
            LoadData();
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


    private NotifyTaskCompletion<ObservableCollection<PostVm>> userPosts;

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
    
    
    
    public ButtonCommand ChangeEmailCommand => new ButtonCommand(o =>
    {
        Account.TryChangeProperty(PersonPropertyChangeType.Email, ChangeEmail, VerifyPassword);
    }, o => Account.AccountPerson is not null && Account.AccountPerson.IsCompleted &&  !string.IsNullOrEmpty(ChangeEmail) && ChangeEmail != Account.AccountPerson.Result.Email);
    public ButtonCommand ChangeNameCommand => new ButtonCommand(o =>
    {
        Account.TryChangeProperty(PersonPropertyChangeType.Username, ChangeName, VerifyPassword);
    }, o => Account.AccountPerson is not null && Account.AccountPerson.IsCompleted  && !string.IsNullOrEmpty(ChangeName) && ChangeName != Account.AccountPerson.Result.Name );
    public ButtonCommand ChangePasswordCommand => new ButtonCommand(o =>
    {
        Account.TryChangeProperty(PersonPropertyChangeType.Password, Utils.HashPassword(ChangePassword), VerifyPassword);
    }, o => Account.AccountPerson is not null && Account.AccountPerson.IsCompleted &&  !string.IsNullOrEmpty(ChangePassword));

    public ButtonCommand SwitchArtistModeCommand => new ButtonCommand(o =>
    {
        Account.TryChangeProperty(PersonPropertyChangeType.IsArtist, !Account.IsArtist, VerifyPassword);
    }, o => Account.AccountPerson is not null && Account.AccountPerson.IsCompleted );
    
    public ButtonCommand ChangeAvatarCommand => new ButtonCommand(o =>
    {
        Account.TryChangeProperty(PersonPropertyChangeType.Image, ChangeAvatar, VerifyPassword);
    }, o => Account.AccountPerson is not null && Account.AccountPerson.IsCompleted && ChangeAvatar != Account.AccountPerson.Result.Image );



    public ButtonCommand SetChangePasswordVisible => new ButtonCommand(o =>
    {
        ChangePasswordVisible = !changePasswordVisible;
    });
    
    public ButtonCommand SetVerifyPasswordVisible => new ButtonCommand(o =>
    {
        VerifyPasswordVisible = !verifyPasswordVisible;
    });

    public ButtonCommand LoadAvatarCommand => new ButtonCommand(o =>
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".jpg";
        dialog.Filter = "Image files |*.jpg;*.png";
        var result = dialog.ShowDialog();
        if (result is not true) return;

        string path = dialog.FileName;

        var image = new BitmapImage(new Uri(path, UriKind.Absolute));

        ChangeAvatar = image;
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
    

    private void LoadData()
    {
        SubscribersCount = new NotifyTaskCompletion<int>(UserDAO.Instance.SubscribersCountAsync(userId));
        SubscriptionsCount = new NotifyTaskCompletion<int>(UserDAO.Instance.SubscriptionsCountAsync(userId));
        UserPosts = new NotifyTaskCompletion<ObservableCollection<PostVm>>(LoadPostList());

        ChangeName = Account.AccountPerson.Result.Name;
        ChangeEmail = Account.AccountPerson.Result.Email;
        ChangeAvatar = Account.AccountPerson.Result.Image;
        ChangePassword = "";
    }

    private void ClearCurrentUserData()
    {
        while (NavigationVm.Instance.Navigation.CanGoBack) NavigationVm.Instance.Navigation.RemoveBackEntry();
    }

    

    public AccountPageVm()
    {
        UserId = Account.AccountPerson.Result.ID;
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}