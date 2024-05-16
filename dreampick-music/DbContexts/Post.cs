using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class Post
{
    public string Id { get; set; }
    
    

    public string UserId { get; set; }
    public User User { get; set; }

    public List<User> Likes { get; set; } = new List<User>();
    
    

    public string Text { get; set; }
    

    public DateTime CreatedOn { get; set; }
    
    public Playlist? Playlist { get; set; }
    
}