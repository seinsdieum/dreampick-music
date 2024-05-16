using System;

namespace dreampick_music.DbContexts;

public class SubscribeRelation
{
    public string SubscriberId { get; set; }
    
    public string FollowerId { get; set; }
    
    public User Subscriber { get; set; }
    public User Follower { get; set; }
    public DateTime CreatedOn { get; set; }= DateTime.Now;
}