using System.Windows;
using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music;

public partial class Person : Page
{
    public Person(string id)
    {
        
        // TODO move to VM
        InitializeComponent();
        if (this.DataContext is PersonVm context)
        {
            context.UserId = id;
            
            /*var person = PlatformDAO.Instance.LoadPerson(id);

            var subscribers = PlatformDAO.Instance.LoadUserSubscribers(id);
            var subscribes = PlatformDAO.Instance.LoadUserSubscriptions(id);
            var posts = PlatformDAO.Instance.LoadUserPosts(id);

            person.Subscribers = subscribers;
            person.Subscribes = subscribes;
            context.UserPosts = posts;
            
            if (!string.IsNullOrEmpty(person.ID)) context.User = person;*/
        }
    }
}