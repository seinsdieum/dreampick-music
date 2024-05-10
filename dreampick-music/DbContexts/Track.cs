using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class Track
{
    [Required]

    public DateTime CreatedOn { get; set; }
    
    [Required]

    public string Id { get; set; }
    
    [Required]

    public string Name { get; set; }
    
    
    [Required]

    public Uri Source { get; set; }
    [Required]
    
    public Playlist Playlist { get; set; }
    
    [Required]

    public List<User> Likes { get; set; }

}