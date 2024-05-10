using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Controls.Ribbon;
using dreampick_music.Models;

namespace dreampick_music;

public class AuthenticationContext : INotifyPropertyChanged
{
    private string loginUsername;
    private string loginPassword;
    private bool passwordVisible = false;

    private string regEmail = "";
    private string regName = "";
    private string regPassword = "";
    private bool regPasswordVisible = false;

    private ObservableCollection<string> validationErrors => new ObservableCollection<string>()
    {
        "Username must have at least 4 letters",
        "Username should have only 1-9, a-Z or (. _) symbols",
        "Invalid email",
        "Invalid email",
    };

    private ObservableCollection<string> regValidationErrors = new ObservableCollection<string>();

    private ObservableCollection<string> loginValidationErrors = new ObservableCollection<string>();
    public ObservableCollection<string> LoginValidationErrors
    {
        get => loginValidationErrors;
        set
        {
            loginValidationErrors = value;
            OnPropertyChanged(nameof(LoginValidationErrors));
        }
    }
    
    public ObservableCollection<string> RegValidationErrors
    {
        get => regValidationErrors;
        set
        {
            regValidationErrors = value;
            OnPropertyChanged(nameof(RegValidationErrors));
        }
    }
    
    

    public bool PasswordVisible
    {
        get => passwordVisible;
        set
        {
            passwordVisible = value;
            OnPropertyChanged(nameof(PasswordVisible));
            OnPropertyChanged(nameof(PasswordNotVisible));
        }
    }

    public bool PasswordNotVisible => !passwordVisible;

    public string LoginUsername
    {
        get => loginUsername;
        set
        {
            loginUsername = value;
            OnPropertyChanged(nameof(LoginUsername));
        }
    }


    public string LoginPassword
    {
        get => loginPassword;
        set
        {
            loginPassword = value;
            OnPropertyChanged(nameof(LoginPassword));
        }
    }

    public ButtonCommand SetPasswordVisible => new ButtonCommand(o =>
    {
        PasswordVisible = !PasswordVisible;
    });



    public string RegName
    {
        get => regName;
        set
        {
            regName = value;
            OnPropertyChanged(nameof(RegName));
        }
    }
    public string RegEmail
    {
        get => regEmail;
        set
        {
            regEmail = value;
            OnPropertyChanged(nameof(RegEmail));
        }
    }

    public string RegPassword
    {
        get => regPassword;
        set
        {
            regPassword = value;
            OnPropertyChanged(nameof(RegPassword));
        }
    }

    //TODO localized errors
    private Func<(bool, string)> RegUsernameCheck => () =>
    {
        var regex = new Regex("^[a-zA-Z_](?!.*?\\.{2})[\\w.]{1,28}[\\w]$");
        if (!(RegName.Length > 3 && regex.IsMatch(RegName))) return (false, "Username not valid");
        
        return AccountDAO.Instance.NameExistingAsync(RegName) ? (false, "Username is existing already") : (true, "");
    };

    private Func<(bool, string)> RegEmailCheck => () =>
    {
        var regex = new Regex("^\\S+@\\S+\\.\\S+$");
        return (regex.IsMatch(RegEmail), "User email not valid");
    };

    private Func<(bool, string)> RegPasswordCheck => () =>
    {
        var regex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,16}$");
        return (regex.IsMatch(RegPassword), "Password:  min8, 1 letter u, 1 letter U, 1 number");
    };

    public AccountVm Account => AccountVm.Instance;

    private bool ValidateRegister()
    {
        var checkList = new List<Func<(bool, string)>>()
        {
            RegEmailCheck,
            RegPasswordCheck,
            RegUsernameCheck,
        };

        var isCheck = true;

        var errorList = new List<string>();

        foreach (var check in checkList)
        {
            var c = check.Invoke();
            if(!c.Item1) errorList.Add(c.Item2);
            isCheck = isCheck && c.Item1;
        }

        RegValidationErrors = new ObservableCollection<string>(errorList);
        
        if (!isCheck) return false;

        return isCheck;
        //  DATABASE TRANSACTION CODE
    }

    private void RefreshForms()
    {
        PasswordVisible = false;
    }

    public ButtonCommand TryRegister => new ButtonCommand(o =>
    {
        if (ValidateRegister())
        {
            Account.TryRegister(new AccountModel()
            {
                Email = RegEmail,
                ID = Utils.GenerateRandomString(30),
                Name = RegName,
                Password = RegPassword
            });
        };
    });

    public ButtonCommand TryLogin => new ButtonCommand(o =>
    {
        if (ValidateLogin())
        {
            Account.TryAuthenticate(LoginUsername, LoginPassword, true);
        }
    });

    private bool ValidateLogin()
    {
        return !string.IsNullOrEmpty(LoginPassword) && !string.IsNullOrEmpty(LoginUsername);
    }
    


    public ButtonCommand CloseApplication => new ButtonCommand(o =>
    {
        App.Current.Shutdown();
    });
    

    #region NotifyPropertyChange Members

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion
}