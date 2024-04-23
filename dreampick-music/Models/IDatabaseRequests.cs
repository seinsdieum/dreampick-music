using System;
using System.Collections.ObjectModel;

namespace dreampick_music.Models;

public interface IDatabaseRequests
{
    
    void AddPost(Post post);
    ObservableCollection<Post> LoadPosts();
    //ObservableCollection<Post> LoadPosts(params Func<bool, bool>[] conditions);

    //  void AddPerson(Person person);
    //  void ChangePerson(Person newPerson, string id);
    Person LoadPerson(string id);
    int LoadUserSubscribers(string id);
    int LoadUserSubscriptions(string id);
    //  void RemovePerson(string id);

    //void AddPlaylist(Playlist playlist);
    //void AddAudio(Track track);
    //Track LoadAudio(string id);
    //Playlist LoadPlaylist(string id);
    //void RemoveAudio(string id);




}