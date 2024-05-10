using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;

namespace dreampick_music;

public class SubscribersVm : INotifyPropertyChanged
{

    private string userId;

    public string UserId
    {
        get
        {
            return userId;
        }
        set
        {
            userId = value;
            
            
            LoadSubsAsync();
        }
    }

    private NotifyTaskCompletion<ObservableCollection<Models.Person>> subs;

    public NotifyTaskCompletion<ObservableCollection<Models.Person>> Subscribers
    {
        get
        {
            return subs;
        }
        set
        {
            subs = value; OnPropertyChanged(nameof(Subscribers));
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
    
    public ButtonCommand NavigateUserCommand => new ButtonCommand(o =>
    {
        if (o is string id && NavigationVm.Instance.Navigation is NavigationService vm)
        {
            vm.Navigate(new Person(id));
        }
    });

    private void LoadSubsAsync()
    {
        Subscribers =
            new NotifyTaskCompletion<ObservableCollection<Models.Person>>(UserDAO.Instance.SubscribersAsync(userId));
    }
    
    private void DestroyObjects()
    {
        Subscribers = null;
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}