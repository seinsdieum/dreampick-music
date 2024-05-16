using System;

namespace dreampick_music.DbContexts;

public class TrackLikeRelation
{
    public User User { get; set; }
    public Track Track { get; set; }

    public string UserId { get; set; }
    public string TrackId { get; set; }
    public DateTime CreatedOn { get; set; }= DateTime.Now;
}