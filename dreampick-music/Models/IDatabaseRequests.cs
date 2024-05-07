using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace dreampick_music.Models;

public interface IDatabaseRequests
{
    Task<bool> AddPost(Post post);
    Task<ObservableCollection<Post>> LoadUserPostsAsync(string id);
    //ObservableCollection<Post> LoadPosts(params Func<bool, bool>[] conditions);

    Task<Person> LoadPersonAsync(string id);


    //  void RemovePerson(string id);

    //void AddPlaylist(Playlist playlist);
    Task AddPlaylist(Playlist playlist);
    // Task AddPlaylistAudio(Playlist playlist);


    //Track LoadAudio(string id);
    //Playlist LoadPlaylist(string id);
    //void RemoveAudio(string id);




}