using System.Windows.Controls;

namespace dreampick_music;

public partial class PublishAudio : Page
{
    public PublishAudio()
    {
        InitializeComponent();
        if (DataContext is PublishAudioVm vm && App.Current.MainWindow.DataContext is MainVm mainVm)
        {
            vm.MainVm = mainVm;
        }
    }
}