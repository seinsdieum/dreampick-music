using System;

namespace dreampick_music.Models;

public class Post
{
    public string Description { get; set; }
    public Person PostAuthor { get; set; }
    public string ID { get; set; }

    public DateTime PublicationDate
    {
        get;
        set;
    }

    public Post(string id, string description = "")
    {
        Description = description;

        if (PostAuthor == null)
        {
            PostAuthor = new User();
            PostAuthor.Name = "alexellipse";
            PostAuthor.ID = "sdfsd";
        }

        ID = id;
        PublicationDate = DateTime.Now;
    }
}