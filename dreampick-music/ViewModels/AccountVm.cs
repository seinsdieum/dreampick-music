using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using dreampick_music.Models;
using MessageBox = System.Windows.Forms.MessageBox;

namespace dreampick_music;

public class AccountVm : INotifyPropertyChanged
{
    public static AccountVm Instance = new AccountVm();

    private int loginAttemtps { get; set; } = 0;

    private bool isAuthorized;


    private NotifyTaskCompletion<Models.Person> accountPerson;

    private bool authenticationInProcess = false;


    public bool IsArtist => accountPerson?.Result is Artist;

    public NotifyTaskCompletion<Models.Person> AccountPerson
    {
        get => accountPerson;
        set
        {
            accountPerson = value;
            OnPropertyChanged(nameof(AccountPerson));
            AddArtistCheck();
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


    public void TryAuthenticate(string name, string password, bool navigateToMain = false)
    {
        AuthenticationInProcess = true;
        loginAttemtps += 1;

        var auth = new NotifyTaskCompletion<bool>(
            AccountDAO.Instance.VerifyUserAsync(name, Utils.HashPassword(password)));

        var synchronizedComplete = false;

        if (!synchronizedComplete && auth.IsCompleted)
        {
            AuthenticationInProcess = false;
            IsAuthorized = auth.Result;
            if (isAuthorized)
            {
                AccountPerson = new NotifyTaskCompletion<Models.Person>(AccountDAO.Instance.LoadAccountAsync(name));

                if (navigateToMain) WindowModel.SwitchToMainWindow();
            }
            else
            {
                AccountPerson = null;
            }

            synchronizedComplete = true;
        }

        if (!synchronizedComplete)
            auth.PropertyChanged += (_, _) =>
            {
                if (auth.IsCompleted)
                {
                    AuthenticationInProcess = false;
                    IsAuthorized = auth.Result;
                    if (isAuthorized)
                    {
                        loginAttemtps = 0;
                        AccountPerson =
                            new NotifyTaskCompletion<Models.Person>(AccountDAO.Instance.LoadAccountAsync(name));


                        if (navigateToMain) WindowModel.SwitchToMainWindow();
                    }
                    else
                    {
                        AccountPerson = null;
                    }

                    auth = null;
                }
            };
    }

    public void TryRegister(AccountModel accountModel)
    {
        var normalPassword = accountModel.Password;
        accountModel.Password = Utils.HashPassword(accountModel.Password);

        var task = new NotifyTaskCompletion<bool>(AccountDAO.Instance.AddUserAsync(accountModel));

        var synchronizedComplete = false;

        if (!synchronizedComplete && task.IsCompleted)
        {
            if (task.Result)
            {
                TryAuthenticate(accountModel.Name, normalPassword, true);
            }
            else Console.WriteLine("hueta");
        }

        if (!synchronizedComplete)
            task.PropertyChanged += (_, _) =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result)
                    {
                        TryAuthenticate(accountModel.Name, normalPassword, true);
                    }
                    else Console.WriteLine("hueta");
                }
            };
    }

    public void TryChangeProperty(PersonPropertyChangeType type, object value, string normalPassword)
    {
        var nameValue = value is string str ? str : "";
        if (type is PersonPropertyChangeType.Username && string.IsNullOrEmpty(nameValue)) return;

        loginAttemtps += 1;

        var auth = new NotifyTaskCompletion<bool>(
            AccountDAO.Instance.VerifyUserAsync(AccountPerson.Result.Name, Utils.HashPassword(normalPassword)));

        var synchronizedComplete = false;


        if (!synchronizedComplete && auth.IsCompleted)
        {
            IsAuthorized = auth.Result;
            if (isAuthorized)
            {
                var task = new NotifyTaskCompletion<bool>(
                    AccountDAO.Instance.ChangePersonProperty(AccountPerson.Result.ID, type, value));
                var synchonizedComplete = false;

                if (!synchonizedComplete && task.IsCompleted)
                {
                    if (task.Result)
                    {
                        AccountPerson =
                            new NotifyTaskCompletion<Models.Person>(
                                AccountDAO.Instance.LoadAccountAsync(type == PersonPropertyChangeType.Username
                                    ? nameValue
                                    : AccountPerson.Result.Name));
                    }
                    else Console.WriteLine("hueta");
                }

                if (!synchonizedComplete)
                    task.PropertyChanged += (sender, args) =>
                    {
                        if (task.IsCompleted)
                        {
                            if (task.Result)
                            {
                                AccountPerson =
                                    new NotifyTaskCompletion<Models.Person>(
                                        AccountDAO.Instance.LoadAccountAsync(AccountPerson.Result.Name));
                            }
                        }
                    };
            }

            synchronizedComplete = true;
        }

        if (!synchronizedComplete)
            auth.PropertyChanged += (_, _) =>
            {
                if (auth.IsCompleted)
                {
                    IsAuthorized = auth.Result;
                    if (isAuthorized)
                    {
                        var task = new NotifyTaskCompletion<bool>(
                            AccountDAO.Instance.ChangePersonProperty(AccountPerson.Result.ID, type, value));
                        var synchonizedComplete = false;

                        if (!synchonizedComplete && task.IsCompleted)
                        {
                            if (task.Result)
                            {
                                AccountPerson =
                                    new NotifyTaskCompletion<Models.Person>(
                                        AccountDAO.Instance.LoadAccountAsync(AccountPerson.Result.Name));
                            }
                            else Console.WriteLine("hueta");
                        }

                        if (!synchonizedComplete)
                            task.PropertyChanged += (sender, args) =>
                            {
                                if (task.IsCompleted)
                                {
                                    if (task.Result)
                                    {
                                        AccountPerson =
                                            new NotifyTaskCompletion<Models.Person>(
                                                AccountDAO.Instance.LoadAccountAsync(AccountPerson.Result.Name));
                                    }
                                }
                            };
                    }


                    auth = null;
                }
            };
    }

    private void AddArtistCheck()
    {
        var synchronized = false;
        if (AccountPerson is not null && AccountPerson.IsCompleted)
        {
            synchronized = true;
            OnPropertyChanged(nameof(IsArtist));
        }

        if (AccountPerson is not null && !synchronized)
        {
            AccountPerson.PropertyChanged += (sender, args) =>
            {
                if (AccountPerson.IsCompleted)
                {
                    OnPropertyChanged(nameof(IsArtist));
                }
            };
        }
    }


    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}