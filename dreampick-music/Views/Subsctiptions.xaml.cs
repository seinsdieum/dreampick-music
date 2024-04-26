using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music;

public partial class Subsctiptions : Page
{
    public Subsctiptions(string id)
    {
        InitializeComponent();
        
        if (DataContext is SubscriptionsVm vm)
        {
            vm.UserId = id;
        }
    }
}