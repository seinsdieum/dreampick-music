using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class User
{

    public string Id { get; set; }

    public string Username { get; set; }
    

    public string Email { get; set; }
    public byte[]? Image { get; set; }
    

    public DateTime CreatedOn { get; set; }

    public List<User> Follows { get; set; } = new List<User>();
    public List<Post> Posts { get; set; } = new List<Post>();
    public List<Post> LikedPosts { get; set; } = new List<Post>();

    public List<User> Subscribers { get; set; } = new List<User>();

    public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    public List<Track> Tracks { get; set; } = new List<Track>();

    public List<Playlist> OwnedPlaylists { get; set; } = new List<Playlist>();
    public bool IsArtist { get; set; }
    
}