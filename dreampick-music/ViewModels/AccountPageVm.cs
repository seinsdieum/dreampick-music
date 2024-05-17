using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;
using Microsoft.Win32;

namespace dreampick_music;

public class AccountPageVm : INotifyPropertyChanged, INotifyDataErrorInfo
{

    public AccountVm Account => AccountVm.Instance;
    
    
    private string  userId = "";

    private BitmapImage changeAvatar;
    private string changeName = "";
    private string changeEmail = "";
    private string changePassword = "";
    private string verifyPassword = "";


    private bool artistIsSet = false;
    private bool changePasswordVisible = false;
    private bool verifyPasswordVisible = false;


    public bool ArtistIsSet
    {
        get => artistIsSet;
        set
        {
            artistIsSet = value;
            OnPropertyChanged(nameof(ArtistIsSet));
            ValidateInfo();
        }
    }
    public BitmapImage ChangeAvatar
    {
        get => changeAvatar;
        set
        {
            changeAvatar = value;
            OnPropertyChanged(nameof(ChangeAvatar));
            ValidateInfo();

        }
    }
    public string ChangeName
    {
        get => changeName;
        set
        {
            changeName = value;
            OnPropertyChanged(nameof(ChangeName));
            ValidateInfo();

        }
    }
    
    public string ChangeEmail
    {
        get => changeEmail;
        set
        {
            changeEmail = value;
            OnPropertyChanged(nameof(ChangeEmail));
            ValidateInfo();

        }
    }
    
    public string ChangePassword
    {
        get => changePassword;
        set
        {
            changePassword = value;
            OnPropertyChanged(nameof(ChangePassword));
            ValidateInfo();

        }
    }
    
    public string VerifyPassword
    {
        get => verifyPassword;
        set
        {
            verifyPassword = value;
            OnPropertyChanged(nameof(VerifyPassword));            ValidateInfo();

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
    
    public ButtonCommand NavigateSubscribersCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                if (o is string id && NavigationVm.Instance.Navigation is NavigationService service)
                {
                    service.Navigate(new UserCollection(id));
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
                    service.Navigate(new UserCollection(id));
                }
            });
        }
    }
    
    
    //TODO validation for changes


    public ButtonCommand SwitchArtistCommand => new ButtonCommand(o =>
    {
        ArtistIsSet = !ArtistIsSet;
    });

    public ButtonCommand SaveChangesCommand => new ButtonCommand(o =>
    {
        UpdateAccountData();

    }, o => accountErrorsChecked && !HasErrors);

    private async Task UpdateAccountData()
    {
        var hashedPassword = "";
        var newAccount = new User()
        {
            CreatedOn = Account.AccountPerson.CreatedOn,
            Email = ChangeEmail,
            Follows = Account.AccountPerson.Follows,
            Subscribers = Account.AccountPerson.Subscribers,
            Posts = Account.AccountPerson.Posts,
            LikedPosts = Account.AccountPerson.LikedPosts,
            Playlists = Account.AccountPerson.Playlists,
            OwnedPlaylists = Account.AccountPerson.OwnedPlaylists,
            Username = ChangeName,
            IsArtist = ArtistIsSet,
            Tracks = Account.AccountPerson.Tracks,
            Id = Account.AccountPerson.Id,
            Image = Utils.GetByteArrayFromImage(ChangeAvatar),
        };
        
        
        var a= await Account.TryUpdate(Account.AccountPerson.Id, Utils.HashPassword(VerifyPassword), newAccount, ChangePassword);

        if (a) Account.AccountPerson = newAccount;
    }


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

        var userRepository = new UserRepository();
        
        var postsAsync = await userRepository.GetUserPosts(AccountVm.Instance.AccountPerson.Id);

        return new ObservableCollection<PostVm>(
            postsAsync.Select(p => new PostVm() { Post = p} )
        );

    }
    

    private void LoadData()
    {

        var db = new UserRepository();
        SubscribersCount = new NotifyTaskCompletion<int>(db.GetSubscribersCount(AccountVm.Instance.AccountPerson.Id));
        SubscriptionsCount = new NotifyTaskCompletion<int>(db.GetFollowersCount(AccountVm.Instance.AccountPerson.Id));
        UserPosts = new NotifyTaskCompletion<ObservableCollection<PostVm>>(LoadPostList());

        ChangeName = Account.AccountPerson.Username;
        ChangeEmail = Account.AccountPerson.Email;
        if(Account.AccountPerson.Image != null) ChangeAvatar = Utils.GetBitmapImage(Account.AccountPerson.Image);
        ChangePassword = "";
    }

    private void ClearCurrentUserData()
    {
        while (NavigationVm.Instance.Navigation.CanGoBack) NavigationVm.Instance.Navigation.RemoveBackEntry();
    }

    

    public AccountPageVm()
    {
        UserId = Account.AccountPerson.Id;
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }



    private Dictionary<string, List<string>> accountErrors = new();
    
    public List<string> AccountErrors => accountErrors.SelectMany(item => item.Value).ToList();
    private bool accountErrorsChecked = false;

    public IEnumerable GetErrors(string? propertyName)
    {
        throw new NotImplementedException();
    }
    
    
    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void ValidateInfo()
    {
        ValidateName();
        ValidateEmail();
        ValidatePassword();
        ValidateVerifyPassword();
        ValidateImage();

        accountErrorsChecked = true;
        
        OnPropertyChanged(nameof(AccountErrors));
    }
    
    private void ValidateImage()
    {
        ClearErrors(nameof(ChangeAvatar));

        
        if(ChangeAvatar is not BitmapImage) AddError(nameof(ChangeAvatar), Utils.GetLocalizedName("LMustAvatar"));
    }

    private void ValidateName()
    {
        ClearErrors(nameof(ChangeName));
        var regex = new Regex("^[a-zA-Z_](?!.*?\\.{2})[\\w.]{1,28}[\\w]$");

        
        if(string.IsNullOrEmpty(ChangeName) || !regex.IsMatch(ChangeName)) AddError(nameof(ChangeName), Utils.GetLocalizedName("LNameInvalid"));
    }

    private void ValidateEmail()
    {
        ClearErrors(nameof(ChangeEmail));
        var regex = new Regex("^\\S+@\\S+\\.\\S+$");
        
        if(string.IsNullOrEmpty(ChangeEmail) || !regex.IsMatch(ChangeEmail)) AddError(nameof(ChangeEmail), Utils.GetLocalizedName("LEmailInvalid"));
    }

    private void ValidatePassword()
    {
        ClearErrors(nameof(ChangePassword));
        var regex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,16}$");
        
        if(!regex.IsMatch(ChangePassword)) AddError(nameof(ChangePassword), Utils.GetLocalizedName("LPasswordMustContain"));
    }

    
    private void ValidateVerifyPassword()
    {
        ClearErrors(nameof(VerifyPassword));
        
        if(string.IsNullOrEmpty(VerifyPassword)) AddError(nameof(VerifyPassword), Utils.GetLocalizedName("LMustVerifyPassword"));
        
    }

    private void AddError(string propertyName, string error)
    {
        
        
        if (!accountErrors.ContainsKey(propertyName))
            accountErrors[propertyName] = new List<string>();

        if (!accountErrors[propertyName].Contains(error))
        {
            accountErrors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    private void ClearErrors(string propertyName)
    {
        if (accountErrors.ContainsKey(propertyName))
        {
            accountErrors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }
    

    public bool HasErrors { get; }
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
}