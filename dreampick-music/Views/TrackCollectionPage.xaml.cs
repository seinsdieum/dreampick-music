using System;
using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music.Views;

public partial class TrackCollectionPage : Page
{
    public TrackCollectionPage(TrackCollectionType type, string referenceId, params Action<object>[] selected)
    {
        InitializeComponent();

        if (DataContext is TrackCollectionVm vm)
        {
            if (selected.Length != 0)
            {
                vm.IsSelection = true;
                vm.AddOnObjectSelected(selected);
            }
            
            vm.CollectionType = type;
            vm.ReferenceId = referenceId;
        }
    }
}