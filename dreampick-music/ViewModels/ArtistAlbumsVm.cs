using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class ArtistAlbumsVm : INotifyPropertyChanged
{
    
    private string artistId;
    public string ArtistId
    {
        set
        {
            artistId = value;
            LoadArtistAlbumRelations();
        }
    }

    private NotifyTaskCompletion<List<string>> idList;

    public NotifyTaskCompletion<List<string>> IdList
    {
        get => idList;
        set
        {
            idList = value;
            LoadAlbums();
        }
    }
    

    private NotifyTaskCompletion<ObservableCollection<Playlist>> albums;

    public NotifyTaskCompletion<ObservableCollection<Playlist>> Albums
    {
        get => albums;
        set
        {
            albums = value; OnPropertyChanged(nameof(Albums));
        }
    }


    private void LoadArtistAlbumRelations()
    {
        IdList = new NotifyTaskCompletion<List<string>>(PlatformDAO.Instance.GetUserPlaylistRelations(artistId));
    }

    private void LoadAlbums()
    {
        Albums = new NotifyTaskCompletion<ObservableCollection<Playlist>>(PlatformDAO.Instance.GetAlbumsInfo(idList.Result));
    }

    public ButtonCommand NavigateAlbumCommand => new ButtonCommand(param =>
    {
        if (param is not string id) return;
        if (NavigationVm.Instance.Navigation is NavigationService service)
        {
            service.Navigate(new EditAlbumPage(id));
        }
    });
    
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}