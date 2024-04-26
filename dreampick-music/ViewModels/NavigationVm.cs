using System;
using System.ComponentModel;
using System.Windows.Navigation;

namespace dreampick_music;

public class NavigationVm : INotifyPropertyChanged
{
    public static NavigationVm Instance = new NavigationVm();

    private NavigationService navigation;

    public NavigationService Navigation
    {
        get => navigation;
        set
        {
            navigation = value;
            OnPropertyChanged(nameof(Navigation));
        }
    }

    public void Navigate(object o)
    {
        if (Navigation != null) Navigation.Navigate(o);
    }

    public void ClearNavigateBack(Action clearContentAction)
    {
        if (navigation.CanGoBack)
        {
            clearContentAction.Invoke();
            navigation.Content = "";
            navigation.GoBack();
        }
    }


    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}