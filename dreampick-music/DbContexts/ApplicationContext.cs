using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace dreampick_music.DbContexts;

public class ApplicationContext : DbContext
{
    public DbSet<User> UsersSet => Set<User>();
    public DbSet<Account> AccountsSet => Set<Account>();
    public DbSet<Playlist> PlaylistsSet => Set<Playlist>();
    public DbSet<Post> PostsSet => Set<Post>();
    public DbSet<Track> TracksSet => Set<Track>();
    public DbSet<LikeRelation> PostLikes => Set<LikeRelation>();
    public DbSet<TrackLikeRelation> TrackLikes => Set<TrackLikeRelation>();
    public DbSet<PlaylistLikeRelation> PlaylistLikes => Set<PlaylistLikeRelation>();

    public DbSet<UserPlaylistTracks> CustomPlaylistTracks => Set<UserPlaylistTracks>();

    public ApplicationContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();

    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Config.Instance.DbString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        
        
        //  PRE CREATED MANY-TO-MANY
        modelBuilder.Entity<User>()
            .HasMany(u => u.LikedPosts)
            .WithMany(p => p.Likes)
            .UsingEntity<LikeRelation>(
                j => j
                    .HasOne(pu => pu.Post)
                    .WithMany()
                    .HasForeignKey(pu => pu.PostId)
                    .OnDelete(DeleteBehavior.Cascade)
                ,
                j => j
                    .HasOne(pu => pu.User)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserId)
                    .OnDelete(DeleteBehavior.ClientCascade)

                ,
                j =>
                {
                    j.HasKey(pu => new { pu.UserId, pu.PostId });
                });
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tracks)
            .WithMany(p => p.Likes)
            .UsingEntity<TrackLikeRelation>(
                j => j
                    .HasOne(pu => pu.Track)
                    .WithMany()
                    .HasForeignKey(pu => pu.TrackId)
                    .OnDelete(DeleteBehavior.Cascade)
                ,
                j => j
                    .HasOne(pu => pu.User)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                ,
                j =>
                {
                    j.HasKey(pu => new { pu.UserId, pu.TrackId });
                });
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Playlists)
            .WithMany(p => p.Likes)
            .UsingEntity<PlaylistLikeRelation>(
                j => j
                    .HasOne(pu => pu.Playlist)
                    .WithMany()
                    .HasForeignKey(pu => pu.PlaylistId)
                    .OnDelete(DeleteBehavior.Cascade)
                ,
                j => j
                    .HasOne(pu => pu.User)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserId)
                    .OnDelete(DeleteBehavior.ClientCascade),
                
                j =>
                {
                    j.HasKey(pu => new { pu.UserId, pu.PlaylistId });
                });
        
        
        // user playlists m2m
        modelBuilder.Entity<Playlist>()
            .HasMany(u => u.UserAddedTracks)
            .WithMany(p => p.MentionedPlaylists)
            .UsingEntity<UserPlaylistTracks>(
                j => j
                    .HasOne(pu => pu.UserTrack)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserTrackId)
                    .OnDelete(DeleteBehavior.Cascade)
                ,
                j => j
                    .HasOne(pu => pu.UserPlaylist)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserPlaylistId)
                    .OnDelete(DeleteBehavior.ClientCascade)

                ,
                j =>
                {
                    j.HasKey(pu => new { pu.UserTrackId, pu.UserPlaylistId });
                });
        
        
        // AUTO-GENERATED MANY-TO-MANY DO NOT TOUCH
        /*modelBuilder.Entity<User>()
            .HasMany(u => u.Tracks)
            .WithMany(t => t.Likes)
            .UsingEntity<Dictionary<string, object>>
            ("TrackUserLikes",
                j => 
                    j.HasOne<Track>()
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade),
                j => 
                    j.HasOne<User>()
                        .WithMany()
                        .OnDelete(DeleteBehavior.ClientCascade)
            )
            ;
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Playlists)
            .WithMany(p => p.Likes)
            .UsingEntity<Dictionary<string, object>>("PlaylistUserRelations",
                j =>
                    j.HasOne<Playlist>()
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne<User>()
                        .WithMany()
                        .OnDelete(DeleteBehavior.ClientCascade)
            );
        
        
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.LikedPosts)
            .WithMany(p => p.Likes)
            .UsingEntity<Dictionary<string, object>>("PostUserLikes",
                j =>
                    j.HasOne<Post>()
                        .WithMany().OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne<User>()
                        .WithMany().OnDelete(DeleteBehavior.ClientCascade)
            )
            ;*/
 
        

        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.MentionedPosts)
            .WithOne(p => p.Playlist)
            .HasForeignKey(p => p.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade)
            ;


        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.Tracks)
            .WithOne(t => t.Playlist)
            .HasForeignKey(t => t.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade)
            ;


        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            ;
        
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.ClientCascade)
            ;
        


        modelBuilder.Entity<User>()
            .HasMany(u => u.OwnedPlaylists)
            .WithOne(p => p.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            ;

        // ошибка существования
        /*modelBuilder.Entity<User>()
            .HasMany(u => u.LikedPosts)
            .WithMany(p => p.Likes)
            .UsingEntity(t => t.ToTable("PostUserLikes"))
            ;*/


        modelBuilder.Entity<User>()
            .HasMany(u => u.Subscribers)
            .WithMany(u => u.Follows)
            .UsingEntity(t => t.ToTable("UserSubscribers"))
            ;

        modelBuilder.Entity<User>()
            .HasMany(u => u.Follows)
            .WithMany(u => u.Subscribers)
            .UsingEntity(t => t.ToTable("UserSubscribers"))
            ;
        
    }
}