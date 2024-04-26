using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music;

public partial class Subscribers : Page
{
    public Subscribers(string id)
    {
        InitializeComponent();

        if (DataContext is SubscribersVm vm)
        {
            vm.UserId = id;
        }
    }
}