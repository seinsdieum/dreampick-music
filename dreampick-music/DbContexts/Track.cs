using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class Track
{

    public DateTime CreatedOn { get; set; }
    

    public string Id { get; set; }
    

    public string Name { get; set; }
    
    

    public Uri Source { get; set; }
    
    public Playlist Playlist { get; set; }
    
    public string PlaylistId { get; set; }
    

    public List<User> Likes { get; set; }

}