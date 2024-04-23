using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Application = System.Windows.Forms.Application;

namespace dreampick_music;

public class PersonVm : INotifyPropertyChanged
{

    private Models.Person user = new Models.User();


    public Models.Person User
    {
        get
        {
            return user;
        }
        set
        {
            user = value; OnPropertyChanged(nameof(User));
        }
    }

    public ButtonCommand BackCommand
    {
        get
        {
            return new ButtonCommand((o =>
            {
                if (App.Current.MainWindow.DataContext is MainVm vm && vm.FrameNavigation is NavigationService service)
                {
                    if(service.CanGoBack) service.GoBack();
                }
            }));
        }
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}