using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class Post
{
    [Required]
    public string Id { get; set; }
    
    [Required]

    public User User { get; set; }
    
    public List<User> Likes { get; set; }
    
    
    [Required]

    public string Text { get; set; }
    
    [Required]

    public DateTime CreatedOn { get; set; }
    
    public Playlist Playlist { get; set; }
    
}