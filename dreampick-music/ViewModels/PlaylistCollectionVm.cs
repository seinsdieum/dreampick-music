using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
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

    private NotifyTaskCompletion<IEnumerable<Playlist>> collection;

    public NotifyTaskCompletion<IEnumerable<Playlist>> Collection
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
        var playlistRepository = new PlaylistRepository();
        
        switch (referenceType)
        {
            case PlaylistCollectionType.Related:
                Collection = new NotifyTaskCompletion<IEnumerable<Playlist>>(playlistRepository.GetByUserId(AccountVm.Instance.AccountPerson.Id));
                break;
            case PlaylistCollectionType.Latest:
                Collection = new NotifyTaskCompletion<IEnumerable<Playlist>>(
                    playlistRepository.GetAll());
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