namespace dreampick_music.DbContexts;

public class Account
{
    
    public string Id { get; set; }
    
    public string HashedPassword { get; set; }

    public bool disabled { get; set; } = false;
}