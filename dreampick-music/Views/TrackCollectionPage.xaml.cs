using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music.Views;

public partial class TrackCollectionPage : Page
{
    public TrackCollectionPage(TrackCollectionType type, string referenceId)
    {
        InitializeComponent();

        if (DataContext is TrackCollectionVm vm)
        {
            vm.CollectionType = type;
            vm.ReferenceId = referenceId;
        }
    }
}