using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using dreampick_music.Models;

namespace dreampick_music;

public class TestPostVm : INotifyPropertyChanged
{
    private  ObservableCollection<Post> posts = new ObservableCollection<Post>();

    public ObservableCollection<Post> Posts
    {
        get
        {
            return posts;
        }
        set
        {
            posts = value; OnPropertyChanged(nameof(Posts));
        }
    }

    public void AddPost(Post post)
    {
        Posts.Add(post);
        OnPropertyChanged(nameof(Posts));
    }
    
    public void RemovePost(Post post)
    {
        Posts.Add(post);
        OnPropertyChanged(nameof(Posts));

    }
    
    public void RemovePost(string id)
    {
        var post = Posts.Where((p => p.ID == id)).ElementAt(0); 
        Posts.Remove(post);
        OnPropertyChanged(nameof(Posts));

    }
    
    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
    
    
    protected  void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler PropertyChanged = delegate { };
}