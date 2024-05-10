using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;

namespace dreampick_music;

public class UserCollectionVm : INotifyPropertyChanged
{
    private UserCollectionType _collectionType = UserCollectionType.NoType;

    public UserCollectionType CollectionType
    {
        set
        {
            _collectionType = value;
            OnPropertyChanged(nameof(CollectionType));
        }
    }
    
    
    private string referenceId;

    public string ReferenceId
    {
        get => referenceId;
        set
        {
            referenceId = value;
            
            
            LoadCollection();
        }
    }

    private NotifyTaskCompletion<ObservableCollection<Models.Person>> collection;

    public NotifyTaskCompletion<ObservableCollection<Models.Person>> Collection
    {
        get
        {
            return collection;
        }
        set
        {
            collection = value; OnPropertyChanged(nameof(Collection));
        }
    } 
    
    
    public ButtonCommand BackCommand => new ((o =>
    {
        if (NavigationVm.Instance is NavigationVm vm)
        {
            vm.ClearNavigateBack(DestroyObjects);
        }
    }));
    
    public ButtonCommand NavigateUserCommand => new (o =>
    {
        if (o is string id && NavigationVm.Instance.Navigation is NavigationService vm)
        {
            vm.Navigate(new Person(id));
        }
    });

    private void LoadCollection()
    {

        Collection = new NotifyTaskCompletion<ObservableCollection<Models.Person>>(_collectionType switch
        {
            UserCollectionType.Subscribers => UserDAO.Instance.SubscribersAsync(referenceId),
            UserCollectionType.Subscriptions => UserDAO.Instance.SubscriptionsAsync(referenceId),
            UserCollectionType.PostLikes => PostDAO.Instance.RelationsAsync(referenceId),
            _ => new Task<ObservableCollection<Models.Person>>(() => new ObservableCollection<Models.Person>()),
        });
    }
    
    private void DestroyObjects()
    {
        Collection = null;
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}