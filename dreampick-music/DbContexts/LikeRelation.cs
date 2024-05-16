using System;

namespace dreampick_music.DbContexts;

public class LikeRelation
{
    public string PostId { get; set; }
    
    public string UserId { get; set; }
    
    public Post Post { get; set; }
    public User User { get; set; }
    public DateTime CreatedOn { get; set; }= DateTime.Now;
}