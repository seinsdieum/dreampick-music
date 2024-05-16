using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using MessageBox = System.Windows.Forms.MessageBox;
using Playlist = dreampick_music.DbContexts.Playlist;
using Post = dreampick_music.DbContexts.Post;
using User = dreampick_music.DbContexts.User;

namespace dreampick_music;

public class AccountVm : INotifyPropertyChanged
{
    public static AccountVm Instance = new AccountVm();

    private int loginAttemtps { get; set; } = 0;

    private bool isAuthorized;
    
    private DbContexts.User accountPerson;

    private bool authenticationInProcess = false;

    public DbContexts.User AccountPerson
    {
        get => accountPerson;
        set
        {
            accountPerson = value;
            OnPropertyChanged(nameof(AccountPerson));
        }
    }

    public bool IsAuthorized
    {
        get => (isAuthorized);
        set
        {
            isAuthorized = value;
            OnPropertyChanged(nameof(IsAuthorized));
            OnPropertyChanged(nameof(IsAuthorizedWhenTried));
        }
    }

    public bool AuthenticationInProcess
    {
        get => authenticationInProcess;
        set
        {
            authenticationInProcess = value;
            OnPropertyChanged(nameof(AuthenticationInProcess));
        }
    }

    public bool IsAuthorizedWhenTried => (isAuthorized || loginAttemtps == 0);

    public async Task TryAuthenticate(string name, string password, bool navigateToMain = false)
    {
        var accountRepo = new AccountRepository();
        try
        {
            AuthenticationInProcess = true;
            loginAttemtps += 1;


            var a = await accountRepo.GetAuthenticatedUser(name, Utils.HashPassword(password));

            if (a.Item1)
            {
                loginAttemtps = 0;
                AccountPerson = a.Item2;
                if (navigateToMain) WindowModel.SwitchToMainWindow();
            }

            AuthenticationInProcess = false;
            IsAuthorized = a.Item1;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
        }
    }
    
    public async Task TryRegister(AccountModel accountModel)
    {
        var accountRepo = new AccountRepository();

        var task = accountRepo.AddUserAccount(new User()
        {
            IsArtist = false,
            CreatedOn = DateTime.Now,
            Id = Utils.GenerateRandomString(10),
            Username = accountModel.Name,
            Email = accountModel.Email,
        }, Utils.HashPassword(accountModel.Password));
        
        
        await Task.WhenAll(task);
        
        
        if (task.Result) await TryAuthenticate(accountModel.Name, accountModel.Password, true);
    }

    public async Task<bool> TryUpdate(string id, string hashedPassword, User account, string newHashedPassword = "")
    {


        AuthenticationInProcess = true;
        loginAttemtps += 1;
        
        var db = new AccountRepository();
        var userDb = new UserRepository();
        
        var a = await db.GetAuthentication(id, hashedPassword);
        
        AuthenticationInProcess = false;
        IsAuthorized = a;
        if (!a) return false;

        loginAttemtps = 0;
        if (!string.IsNullOrEmpty(newHashedPassword))
        {
            await db.Update(new Account()
            {
                disabled = false,
                HashedPassword = newHashedPassword,
                Id = account.Id
            });
        }

        await userDb.Update(account);

        return true;

    }
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}