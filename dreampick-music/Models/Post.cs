using System;

namespace dreampick_music.Models;

public class Post
{
    public string Description { get; set; }
    public Person PostAuthor { get; set; }
    public string ID { get; set; }
    
    public int Likes { get; set; }

    public DateTime PublicationDate
    {
        get;
        set;
    }
    
    public Playlist PostPlaylist { get; set; }

    public Post(string id, string description = "")
    {
        Description = description;

        ID = id;
        PublicationDate = DateTime.Now;
    }

    public Post()
    {
        
    }
}