using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music.Views;

public partial class PlaylistCollection : Page
{
    public PlaylistCollection(PlaylistCollectionType type, string referenceId)
    {
        InitializeComponent();

        if (DataContext is PlaylistCollectionVm vm)
        {
            vm.ReferenceType = type;
            vm.ReferenceId = referenceId;
        }
    }
}