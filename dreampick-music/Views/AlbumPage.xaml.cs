using System.Windows.Controls;

namespace dreampick_music.Views;

public partial class AlbumPage : Page
{
    public AlbumPage(string id)
    {
        InitializeComponent();
        if (DataContext is AlbumPageVm vm)
        {
            vm.AlbumId = id;
        }
    }
}