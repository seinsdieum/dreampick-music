using System.Windows;
using System.Windows.Controls;

namespace dreampick_music;

public partial class Settings : Page
{
    public Settings()
    {
        InitializeComponent();
    }
    
    private void ChangeTheme_OnClick(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        app.ChangeTheme("Resources/Theme/Theme.Purple.xaml");
    }

    private void ChangeLocalization_OnClick(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        app.ChangeLocalization("Resources/Localization/Local.Ru.xaml");
    }
}