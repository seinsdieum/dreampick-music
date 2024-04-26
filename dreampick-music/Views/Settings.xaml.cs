using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace dreampick_music;

public partial class Settings : Page
{
    public Settings()
    {
        InitializeComponent();
    }
    

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("message!");
    }
}