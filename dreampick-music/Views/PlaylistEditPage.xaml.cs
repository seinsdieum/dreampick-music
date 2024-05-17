using System.Windows.Controls;

namespace dreampick_music.Views;

public partial class PlaylistEditPage : Page
{
    public PlaylistEditPage(string id = "")
    {
        InitializeComponent();
        if (DataContext is PlaylistEditPageVm vm) vm.ReferenceId = id;
    }
}