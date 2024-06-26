﻿using System.Windows.Controls;

namespace dreampick_music.Views;

public partial class AlbumPage : Page
{
    public AlbumPage(string id, bool isCustom = false)
    {
        InitializeComponent();
        if (DataContext is AlbumPageVm vm)
        {
            vm.IsCustom = isCustom;
            vm.AlbumId = id;
        }
    }
}