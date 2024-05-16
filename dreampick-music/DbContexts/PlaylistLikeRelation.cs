using System;

namespace dreampick_music.DbContexts;

public class PlaylistLikeRelation
{
    public string PlaylistId { get; set; }
    
    public string UserId { get; set; }
    
    public Playlist Playlist { get; set; }
    public User User { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}