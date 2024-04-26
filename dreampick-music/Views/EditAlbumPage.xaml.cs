using System.Windows.Controls;

namespace dreampick_music.Views;

public partial class EditAlbumPage : Page
{
    public EditAlbumPage(string id)
    {
        InitializeComponent();
        if (DataContext is not EditAlbumVm vm) return;
        vm.AlbumId = id;
        if (App.Current.MainWindow.DataContext is MainVm mainVm) vm.MainVm = mainVm;
    }
}