using System.Windows.Controls;

namespace dreampick_music.Views;

public partial class PlaylistPage : Page
{
    public PlaylistPage(string id)
    {
        InitializeComponent();
        if (DataContext is AlbumPageVm vm)
        {
            vm.AlbumId = id;
        }
    }
}