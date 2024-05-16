using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Navigation;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class ArtistAlbumsVm : INotifyPropertyChanged
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

    private async void LoadAlbums()
    {

        var repository = new PlaylistRepository();
        
        var a = repository.GetAllByArtist(AccountVm.Instance.AccountPerson.Id);
        await a;

        if (a.IsFaulted)
        {
            return;
        }


        Albums = a.Result.ToList();
    }

    public ButtonCommand NavigateAlbumCommand => new ButtonCommand(param =>
    {
        if (param is not string id) return;
        if (NavigationVm.Instance.Navigation is NavigationService service)
        {
            service.Navigate(new EditAlbumPage(id));
        }
    });

    public ButtonCommand NavigateAlbumCreationCommand => new ButtonCommand(param =>
    {
        NavigationVm.Instance.Navigate(new PublishAudio());
        ;
    });

    public ButtonCommand RefreshCommand => new ButtonCommand(o => { LoadAlbums(); });

    public ArtistAlbumsVm()
    {
        LoadAlbums();
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}