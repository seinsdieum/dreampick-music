using System.Windows.Controls;

namespace dreampick_music.Views;

public partial class ArtistAlbums : Page
{
    public ArtistAlbums(string artistId)
    {
        InitializeComponent();

        if (DataContext is ArtistAlbumsVm vm)
        {
            vm.ArtistId = artistId;
        }
    }
}