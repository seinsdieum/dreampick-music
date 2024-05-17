using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class PlaylistCollectionVm : SelectedBase, INotifyPropertyChanged
{

    private string searchString = "";

    public string SearchString
    {
        get => searchString;
        set
        {
            searchString = value;
            OnPropertyChanged(nameof(SearchString));
            SearchPlaylists();
        }
    }

    private List<Playlist> visiblePlaylists = new List<Playlist>();

    public List<Playlist> VisiblePlaylists
    {
        get => visiblePlaylists;
        set
        {
            visiblePlaylists = value;
            OnPropertyChanged(nameof(VisiblePlaylists));
        }
    }

    private Func<Playlist, bool> TextSearchCriteria => playlist => string.IsNullOrEmpty(searchString) || playlist.Name.ToLower().Contains(searchString);

    private List<Playlist> GetSearchResults(List<Func<Playlist, bool>> criterias)
    {
        return collection?.Result is null ? new List<Playlist>() : collection.Result.Where(x => criterias.All(c => c.Invoke(x))).ToList();
    }

    private void SearchPlaylists()
    {
        var list = new List<System.Func<Playlist, bool>>();

        list.Add(TextSearchCriteria);
        VisiblePlaylists = GetSearchResults(list);
    }
    


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
            OnPropertyChanged(nameof(IsSelection));
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

    private async void LoadData()
    {
        var playlistRepository = new PlaylistRepository();
        
        switch (referenceType)
        {
            case PlaylistCollectionType.Related:
                Collection = new NotifyTaskCompletion<IEnumerable<Playlist>>(playlistRepository.GetByUserId(AccountVm.Instance.AccountPerson.Id));
                
                await Collection.Task;
                if (Collection.IsSuccessfullyCompleted) VisiblePlaylists = Collection.Result.ToList();

                
                
                break;
            case PlaylistCollectionType.Latest:
                Collection = new NotifyTaskCompletion<IEnumerable<Playlist>>(
                    playlistRepository.GetAll());
                
                await Collection.Task;
                if (Collection.IsSuccessfullyCompleted) VisiblePlaylists = Collection.Result.ToList();

                
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
        if (o is not Playlist p) return;
        if (IsSelection)
        {
            SelectionCommand.Execute(p.Id);
        }
        else if (NavigationVm.Instance.Navigation is NavigationService service)
        {
            service.Navigate(new AlbumPage(p.Id, p.IsUserPlaylist));
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