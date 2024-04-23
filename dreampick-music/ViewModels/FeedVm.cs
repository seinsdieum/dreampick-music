using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using dreampick_music.Models;

namespace dreampick_music;

public class FeedVm : INotifyPropertyChanged
{
    


    public ObservableCollection<Post> Posts
    {
        get
        {
            return new ObservableCollection<Post>(Models.PlatformDAO.Instance.LoadPosts());
        }
        set
        {
            OnPropertyChanged(nameof(Posts));
        }
    }

    public ButtonCommand NavigateUserCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                if (o is string id && App.Current.MainWindow.DataContext is MainVm vm)
                {
                    vm.FrameNavigation.Navigate(new Person(id));
                }
            });
        }
    }
    


    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    
    

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}