using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
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

    private NotifyTaskCompletion<IEnumerable<User>> collection;

    public NotifyTaskCompletion<IEnumerable<User>> Collection
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

        var userRepository = new UserRepository();
        var postRepository = new PostRepository();
        
        Collection = new NotifyTaskCompletion<IEnumerable<User>>(_collectionType switch
        {
            UserCollectionType.Subscribers => userRepository.GetSubscribers(referenceId),
            UserCollectionType.Subscriptions => userRepository.GetFollowers(referenceId),
            UserCollectionType.PostLikes => postRepository.GetLikes(referenceId),
            _ => new Task<IEnumerable<User>>(() => new List<User>()),
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