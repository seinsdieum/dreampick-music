namespace dreampick_music.DbContexts;

public class Account
{
    public User User { get; set; }
    
    public string HashedPassword { get; set; }
}