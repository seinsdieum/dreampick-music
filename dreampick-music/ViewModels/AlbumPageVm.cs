using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.Models;

namespace dreampick_music;

public class AlbumPageVm : INotifyPropertyChanged
{
    #region VmContext

    private MainVm _mainVm;

    public MainVm MainVm
    {
        set => _mainVm = value;
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
    

    #endregion

    

    private string albumid;

    public string AlbumId
    {
        set
        {
            albumid = value;
            LoadAlbum();
        }
    }

    private NotifyTaskCompletion<Playlist> album;

    public NotifyTaskCompletion<Playlist> Album
    {
        get => album;
        set
        {
            album = value;
            OnPropertyChanged(nameof(Album));
        }
    }

    private void LoadAlbum()
    {
        Album = new NotifyTaskCompletion<Playlist>(PlatformDAO.Instance.LoadAlbumAsync(albumid));
    }

    public ButtonCommand PlayAlbumCommand => new ButtonCommand((o =>
    {
        if (o is not string id) return;
        PlayerVm.Instance.PlayNewQueue(album.Result, id);

    }));

    private void DestroyObjects()
    {
        Album = null;
        albumid = null;
    }


    # region PropertyEvents

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    # endregion
}