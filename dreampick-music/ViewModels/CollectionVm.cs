using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class CollectionVm : INotifyPropertyChanged
{

    private NotifyTaskCompletion<ObservableCollection<Playlist>> albums;

    public NotifyTaskCompletion<ObservableCollection<Playlist>> Albums
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



    public CollectionVm()
    {
        Albums = new NotifyTaskCompletion<ObservableCollection<Playlist>>(PlatformDAO.Instance.LoadAlbumsInfoAsync());
    }
    
    
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}