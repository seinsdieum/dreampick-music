using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbContexts;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Track> Tracks => Set<Track>();

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=D:\\\\MusicPlatform.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Playlist>()
            .HasMany(e => e.Tracks)
            .WithOne(t => t.Playlist)
            .IsRequired();

        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.Likes).WithOne().IsRequired();

        modelBuilder.Entity<Track>()
            .HasMany(p => p.Likes).WithOne().IsRequired();

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Likes).WithOne().IsRequired();

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .IsRequired();
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Playlist).WithOne();


        modelBuilder.Entity<User>()
            .HasMany(u => u.Follows)
            .WithOne().IsRequired()
            ;
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Subscribers)
            .WithOne().IsRequired()
            ;
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Playlists)
            .WithOne().IsRequired()
            ;

    }
}