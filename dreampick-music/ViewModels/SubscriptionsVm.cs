using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;

namespace dreampick_music;

public class SubscriptionsVm : INotifyPropertyChanged
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

    public NotifyTaskCompletion<ObservableCollection<Models.Person>> Subscribes
    {
        get
        {
            return subs;
        }
        set
        {
            subs = value; OnPropertyChanged(nameof(Subscribes));
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
    
    public ButtonCommand NavigateUserCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                if (o is string id && NavigationVm.Instance.Navigation is NavigationService vm)
                {
                    
                    vm.Navigate(new Person(id));
                }
            });
        }
    }
    
    private void DestroyObjects()
    {
        Subscribes = null;
    }

    private void LoadSubsAsync()
    {
        Subscribes =
            new NotifyTaskCompletion<ObservableCollection<Models.Person>>(
                UserDAO.Instance.SubscriptionsAsync(userId));
    }
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}