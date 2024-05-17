using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;

namespace dreampick_music;

public class TrackCollectionVm : SelectedBase, INotifyPropertyChanged
{


    private string textSearch = "";

    public string TextSearch
    {
        get => textSearch;
        set
        {
            textSearch = value;
            OnPropertyChanged(nameof(TextSearch));
            SearchResults();
        }
    }

    private Func<TrackListenVm, bool> TextSearchCriteria => 
        vm => string.IsNullOrEmpty(textSearch) || vm.Track.Name.ToLower().Contains(textSearch) || vm.Track.Playlist.Name.ToLower().Contains(textSearch)
        || vm.Track.Playlist.User.Username.ToLower().Contains(textSearch) || (vm.Track.Playlist.User.Username + " " + vm.Track.Name).ToLower().Contains(textSearch);


    private ObservableCollection<TrackListenVm> GetSearchResults(List<Func<TrackListenVm, bool>> criterias)
    {
        if (Tracks?.Result is null) return new ObservableCollection<TrackListenVm>();

        return new ObservableCollection<TrackListenVm>(Tracks.Result.Where(x => criterias.All(c => c.Invoke(x))));
    }

    private void SearchResults()
    {
        var list = new List<Func<TrackListenVm, bool>>();
        list.Add(TextSearchCriteria);

        VisibleTracks = GetSearchResults(list);
        
        CollectionPlaylist = new Playlist()
        {
            Tracks = VisibleTracks.Select(t => t.Track).ToList(),
        };

    }

    public ObservableCollection<TrackListenVm> VisibleTracks
    {
        get => visibleTracks;
        set
        {
            visibleTracks = value;
            OnPropertyChanged(nameof(VisibleTracks));
        }
    }

    private ObservableCollection<TrackListenVm> visibleTracks = new ObservableCollection<TrackListenVm>();
    
    
    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));

    private void DestroyObjects()
    {
        tracks = null;
        referenceId = null;
    }


    private TrackCollectionType _collectionType;

    public TrackCollectionType CollectionType
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
        set
        {
            referenceId = value;
            Tracks = new NotifyTaskCompletion<ObservableCollection<TrackListenVm>>(LoadTracks());
            OnPropertyChanged(nameof(IsSelection));
        }
    }

    private NotifyTaskCompletion<ObservableCollection<TrackListenVm>> tracks;

    public NotifyTaskCompletion<ObservableCollection<TrackListenVm>> Tracks
    {
        get => tracks;
        set
        {
            tracks = value;
            OnPropertyChanged(nameof(Tracks));
        }
    }
    
    
    public ButtonCommand PlayAlbumCommand => new ButtonCommand((o =>
    {
        if (o is not string id) return;
        PlayerVm.Instance.PlayNewQueue(collectionPlaylist, id);
        OnPropertyChanged(nameof(CollectionPlaylist));
    }));

    private Playlist collectionPlaylist;

    public Playlist CollectionPlaylist
    {
        get => collectionPlaylist;
        set
        {
            collectionPlaylist = value;
            OnPropertyChanged(nameof(CollectionPlaylist));
        }
    }


    public PlayerVm Player => PlayerVm.Instance;


    private async Task<ObservableCollection<TrackListenVm>> LoadTracks()
    {

        var repo = new TrackRepository();
        var trackss = new ObservableCollection<TrackListenVm>();
        var simpleTracks = new List<Track>();
        switch (_collectionType)
        {
            case TrackCollectionType.Queue:
                simpleTracks = PlayerVm.Instance.Queue.Tracks;
                break;
            case TrackCollectionType.Liked:
                simpleTracks = (await repo.GetByUserId(AccountVm.Instance.AccountPerson.Id)).ToList();
                break;
            case TrackCollectionType.Recommended:
                simpleTracks = (await UserTrackRecommends.GetRecommendedTrackCollection(AccountVm.Instance.AccountPerson.Id, 10, 3)).ToList();
                break;
            case TrackCollectionType.NoType:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        

        trackss = new ObservableCollection<TrackListenVm>(simpleTracks.Select(t => new TrackListenVm
        {
            Track = t
        }));

        VisibleTracks = trackss;
        

        CollectionPlaylist = new Playlist()
        {
            Tracks = simpleTracks,
        };

        return trackss;

    }

    public ButtonCommand TrySelectCommand => new ButtonCommand(o =>
    {
        if(!IsSelection) return;
        if(o is string id) SelectionCommand.Execute(o);
    });


    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}