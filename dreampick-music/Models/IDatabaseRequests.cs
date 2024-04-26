using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace dreampick_music.Models;

public interface IDatabaseRequests
{
    // TODO to async all
    Task<bool> AddPost(Post post);
    Task<ObservableCollection<Post>> LoadPostsAsync();
    Task<ObservableCollection<Post>> LoadUserPostsAsync(string id);
    //ObservableCollection<Post> LoadPosts(params Func<bool, bool>[] conditions);

    //  void AddPerson(Person person); TODO
    //  void ChangePerson(Person newPerson, string id); TODO
    Task<Person> LoadPersonAsync(string id);
    Task<int> LoadUserSubscribersAsync(string id);
    Task<int> LoadUserSubscriptionsAsync(string id);

    Task<ObservableCollection<Person>> LoadSubscribersAsync(string id);
    Task<ObservableCollection<Person>> LoadSubscriptionsAsync(string id);

    //  void RemovePerson(string id);

    //void AddPlaylist(Playlist playlist);
    Task AddPlaylist(Playlist playlist);
    // Task AddPlaylistAudio(Playlist playlist);


    //Track LoadAudio(string id);
    //Playlist LoadPlaylist(string id);
    //void RemoveAudio(string id);




}