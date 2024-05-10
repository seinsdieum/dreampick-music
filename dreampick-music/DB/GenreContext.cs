
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace dreampick_music.DB;


public class Genre
{
    private string Id { get; set; }
}

public class GenreContext : DbContext
{
    public DbSet<Genre> Genres => Set<Genre>();

    public GenreContext()
    {
        Database.EnsureCreated();

    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=D:\\\\Music_Platform.db");
    }

}