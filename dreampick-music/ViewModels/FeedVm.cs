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
            return new ObservableCollection<Post>(Models.Posts.LoadPosts());
        }
        set
        {
            OnPropertyChanged(nameof(Posts));
        }
    }
    


    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public FeedVm()
    {
    }
    

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}