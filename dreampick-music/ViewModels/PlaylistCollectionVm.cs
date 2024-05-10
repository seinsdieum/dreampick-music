using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.DB;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class PlaylistCollectionVm : SelectedBase, INotifyPropertyChanged
{

    private PlaylistCollectionType referenceType = PlaylistCollectionType.NoType;
    
    private string referenceId;

    public PlaylistCollectionType ReferenceType
    {
        set => referenceType = value;
    }
    
    public string ReferenceId
    {
        set
        {
            referenceId = value;
            OnPropertyChanged(nameof(ReferenceId));
            LoadData();
        }
    }

    private NotifyTaskCompletion<ObservableCollection<Playlist>> collection;

    public NotifyTaskCompletion<ObservableCollection<Playlist>> Collection
    {
        get => collection;
        set
        {
            collection = value;
            OnPropertyChanged(nameof(Collection));
        }
    }

    private void LoadData()
    {
        switch (referenceType)
        {
            case PlaylistCollectionType.Related:
                Collection = new NotifyTaskCompletion<ObservableCollection<Playlist>>(
                    PlaylistDAO.Instance.RelatedAsync(AccountVm.Instance.AccountPerson.Result.ID));
                break;
            case PlaylistCollectionType.Latest:
                Collection = new NotifyTaskCompletion<ObservableCollection<Playlist>>(
                    PlaylistDAO.Instance.CollectionAsync());
                break;
            case PlaylistCollectionType.Picks:
            case PlaylistCollectionType.NoType:
            default:
                break;
        }
    }
    
    
    #region VmContext
    
    
    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));
    
    public ButtonCommand NavigateAlbumCommand => new ButtonCommand((o =>
    {
        if (o is not string id) return;
        if (IsSelection)
        {
            SelectionCommand.Execute(o);
        }
        else if (NavigationVm.Instance.Navigation is NavigationService service)
        {
            service.Navigate(new AlbumPage(id));
        }

    }));

    private void DestroyObjects()
    {
        collection = null;
    }
    
    

    #endregion
    
    #region NotifyProperty Members

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion
}