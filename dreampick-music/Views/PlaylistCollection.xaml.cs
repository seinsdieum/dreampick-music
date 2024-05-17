using System;
using System.Windows;
using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music.Views;

public partial class PlaylistCollection : Page
{
    public PlaylistCollection(PlaylistCollectionType type, string referenceId, params Action<object>[] selected)
    {
        InitializeComponent();

        if (DataContext is PlaylistCollectionVm vm)
        {
            if (selected.Length != 0)
            {
                vm.IsSelection = true;
                vm.AddOnObjectSelected(selected);
            }
            
            vm.ReferenceType = type;
            vm.ReferenceId = referenceId;
        }
    }
}