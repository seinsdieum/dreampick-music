using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dreampick_music.DbContexts;

public class User
{
    [Required]

    public string Id { get; set; }
    [Required]

    public string Username { get; set; }
    
    [Required]

    public string Email { get; set; }
    public byte[] Image { get; set; }
    
    [Required]

    public DateTime CreatedOn { get; set; }
    
    public List<User> Follows { get; set; }
    public List<Post> Posts { get; set; }
    
    public List<User> Subscribers { get; set; }
    
}