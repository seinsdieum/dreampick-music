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

public class CollectionVm : INotifyPropertyChanged
{


    private List<DbContexts.Playlist> albums;

    public List<DbContexts.Playlist> Albums
    {
        get => albums;
        set
        {
            albums = value;
            OnPropertyChanged(nameof(Albums));
        }
    }



    public ButtonCommand NavigateAlbumCommand => new ButtonCommand((o =>
    {
        if (o is string id && NavigationVm.Instance.Navigation is NavigationService service)
        {
            service.Navigate(new AlbumPage(id));
        }
    }));

    public ButtonCommand RefreshCommand => new ButtonCommand((o =>
    {
        RefreshContent();
    }));

    public ButtonCommand NavigateLikedTracksCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.Navigate(new TrackCollectionPage(TrackCollectionType.Liked, ""));
    });
    
    public ButtonCommand NavigateRecommendedTracksCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.Navigate(new TrackCollectionPage(TrackCollectionType.Recommended, ""));
    });
    
    public ButtonCommand NavigateLikedPlaylistsCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.Navigate(new PlaylistCollection(PlaylistCollectionType.Related, ""));
    });

    public ButtonCommand NavigateLastReleasesCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.Navigate(new PlaylistCollection(PlaylistCollectionType.Latest, ""));
    });
    

    private async Task RefreshContent()
    {
        var playlistRepository = new PlaylistRepository();
        
        var a = await playlistRepository.GetSome(5);
        Albums = a.ToList();
    }



    public CollectionVm()
    {
        RefreshContent();
    }
    
    
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}