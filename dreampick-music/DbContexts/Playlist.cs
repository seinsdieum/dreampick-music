using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class Playlist
{

    
    public string Name { get; set; }
    public string Id { get; set; }
    

    public User User { get; set; }
    public string UserId { get; set; }
    
    public byte[]? Image { get; set; }

    public string Description { get; set; }
    

    public DateTime CreatedOn { get; set; }
    

    public Genre Genre { get; set; }


    public List<Track> Tracks { get; set; } = new List<Track>();


    public List<User> Likes { get; set; } = new List<User>();

    public List<Post> MentionedPosts { get; set; } = new List<Post>();
}