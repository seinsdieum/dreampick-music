using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class Playlist
{
    [Required]

    public string Id { get; set; }
    
    [Required]

    public User User { get; set; }
    
    public byte[] Image { get; set; }

    public string Description { get; set; }
    
    [Required]

    public DateTime CreatedOn { get; set; }
    
    [Required]

    public Genre Genre { get; set; }
    
    [Required]

    public List<Track> Tracks { get; set; }
    
    [Required]

    public List<User> Likes { get; set; }
}