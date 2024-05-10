using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using dreampick_music.Models;

namespace dreampick_music;

public partial class SelectionDialog : Window
{
    
    
    
    public SelectionDialog(object selectionPage)
    {
        InitializeComponent();
        SelectionFrame.Navigate(selectionPage);
        if (selectionPage is Page page)
        {
            DataContext = page.DataContext;
        }
        if (DataContext is SelectedBase vm)
        {
            vm.AddOnSelected(Close);
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if(DataContext is SelectedBase vm) vm.SelectionCommand.Execute(null);
    }
}