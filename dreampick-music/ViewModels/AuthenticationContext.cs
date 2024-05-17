using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.ValidationRules;

namespace dreampick_music;

public class AuthenticationContext : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _loginErrors = new();
    private readonly Dictionary<string, List<string>> _regErrors = new();

    private bool validationLoading = false;

    public bool ValidationLoading
    {
        get => validationLoading;
        set
        {
            validationLoading = value;
            OnPropertyChanged(nameof(ValidationLoading));
        }
    }

    public List<string> RegErrors => _regErrors.SelectMany(item => item.Value).ToList();
    public List<string> LoginErrors  => _loginErrors.SelectMany(item => item.Value).ToList();

    private string loginUsername;
    private string loginPassword;
    private bool passwordVisible = false;


    private string regEmail = "";
    private string regName = "";
    private string regPassword = "";
    private bool regPasswordVisible = false;


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
            ValidateLoginForm();

        }
    }


    public string LoginPassword
    {
        get => loginPassword;
        set
        {
            loginPassword = value;
            OnPropertyChanged(nameof(LoginPassword));
            ValidateLoginForm();
        }
    }

    public ButtonCommand SetPasswordVisible => new ButtonCommand(o => { PasswordVisible = !PasswordVisible; });


    private void ValidateNameToRegister()
    {
        var regex = new Regex("^[a-zA-Z_](?!.*?\\.{2})[\\w.]{1,28}[\\w]$");

        if (string.IsNullOrEmpty(RegName)) AddRegisterError(nameof(RegName), Utils.GetLocalizedName("LNameEmpty"));
        else if (!regex.IsMatch(RegName)) AddRegisterError(nameof(RegName), "name not valid");
    }

    private void ValidateLoginName()
    {
        ClearLoginErrors(nameof(LoginUsername));
        if(string.IsNullOrEmpty(loginUsername)) AddLoginError(nameof(LoginUsername), Utils.GetLocalizedName("LNameEmpty"));
    }

    private void ValidateLoginPassword()
    {
        ClearLoginErrors(nameof(LoginPassword));
        if(string.IsNullOrEmpty(loginPassword)) AddLoginError(nameof(LoginPassword), Utils.GetLocalizedName("LPasswordMustContain"));
    }

    private void ValidateLoginForm()
    {
        logErrorsChecked = true;
        
        ValidateLoginName();
        ValidateLoginPassword();
        OnPropertyChanged(nameof(LoginErrors));
    }

    private void ValidateRegForm()
    {
        ClearRegisterErrors(nameof(RegName));
        

        regErrorsChecked = true;
        ValidateNameToRegister();
        ValidateEmail();
        ValidatePassword();
        
        OnPropertyChanged(nameof(RegErrors));
    }

    public string RegName
    {
        get => regName;
        set
        {
            regName = value;
            OnPropertyChanged(nameof(RegName));
            ValidateRegForm();
        }
    }

    private void ValidateEmail()
    {
        var regex = new Regex("^\\S+@\\S+\\.\\S+$");
        
        ClearRegisterErrors(nameof(RegEmail));
        
        if(string.IsNullOrEmpty(RegEmail)) AddRegisterError(nameof(RegEmail), Utils.GetLocalizedName("LEmailInvalid"));
        if(!regex.IsMatch(RegEmail)) AddRegisterError(nameof(RegEmail), Utils.GetLocalizedName("LEmailInvalid"));
        
    }

    private void ValidatePassword()
    {
        
        ClearRegisterErrors(nameof(RegPassword));

        
        var regex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,16}$");
        if(!regex.IsMatch(RegPassword)) AddRegisterError(nameof(RegPassword), 
            Utils.GetLocalizedName("LPasswordMustContain"));
    }

    private void ValidateExistingUsername()
    {
        var repo = new AccountRepository();
        var a =  repo.Exists(RegName);

        a.Wait();
        
        if(a.Result) AddRegisterError(nameof(RegName), Utils.GetLocalizedName("LNameExisting"));

    }
    

    public string RegEmail
    {
        get => regEmail;
        set
        {
            regEmail = value;
            OnPropertyChanged(nameof(RegEmail));
            ValidateRegForm();
        }
    }

    public string RegPassword
    {
        get => regPassword;
        set
        {
            regPassword = value;
            OnPropertyChanged(nameof(RegPassword));
            ValidateRegForm();
        }
    }

    //TODO localized errors
    private async Task<(bool, string)> RegUsernameCheck()
    {
        var accountRepository = new AccountRepository();

        var regex = new Regex("^[a-zA-Z_](?!.*?\\.{2})[\\w.]{1,28}[\\w]$");
        if (!(RegName.Length > 3 && regex.IsMatch(RegName))) return (false, Utils.GetLocalizedName("LNameInvalid"));

        var a = await accountRepository.Exists(RegName);

        return a ? (false, Utils.GetLocalizedName("LNameExisting")) : (true, "");
    }

    private async Task CheckRegister()
    {

        
        var nameCheck = await RegUsernameCheck();
        var isCheck = nameCheck.Item1;

        if (!isCheck)
        {
            AddRegisterError(nameof(RegName), Utils.GetLocalizedName("LNameExisting"));
            OnPropertyChanged(nameof(RegErrors));
            return;
        }

        Account.TryRegister(new AccountModel()
        {
            Email = RegEmail,
            ID = Utils.GenerateRandomString(30),
            Name = RegName,
            Password = RegPassword
        });
    }

    public AccountVm Account => AccountVm.Instance;
    

    public ButtonCommand TryRegister => 
        new ButtonCommand(o =>
        {
            CheckRegister(); 
        }, o
            => regErrorsChecked && !ValidationLoading && !_regErrors.Any());

    public ButtonCommand TryLogin => new ButtonCommand(o =>
    {
        Account.TryAuthenticate(LoginUsername, LoginPassword, true);
    }, o => logErrorsChecked && !_loginErrors.Any() );
    


    public ButtonCommand CloseApplication => new ButtonCommand(o => { App.Current.Shutdown(); });


    #region NotifyPropertyChange Members

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion

    public IEnumerable GetErrors(string propertyName)
    {
        return _loginErrors.ContainsKey(propertyName) ? _loginErrors[propertyName] : null;
    }


    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void AddLoginError(string propertyName, string error)
    {
        
        
        if (!_loginErrors.ContainsKey(propertyName))
            _loginErrors[propertyName] = new List<string>();

        if (!_loginErrors[propertyName].Contains(error))
        {
            _loginErrors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    private void ClearLoginErrors(string propertyName)
    {
        if (_loginErrors.ContainsKey(propertyName))
        {
            _loginErrors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }


    private void AddRegisterError(string propertyName, string error)
    {
        if (!_regErrors.ContainsKey(propertyName))
            _regErrors[propertyName] = new List<string>();

        if (!_regErrors[propertyName].Contains(error))
        {
            _regErrors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    private void ClearRegisterErrors(string propertyName)
    {
        if (_regErrors.ContainsKey(propertyName))
        {
            _regErrors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    private bool logErrorsChecked = false;
    private bool regErrorsChecked = false;

    public bool HasErrors => _loginErrors.Any() || _regErrors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };
}