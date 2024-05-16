
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbRepositories;

public class UserRepository : 
    IUserRepository

{
    public async Task<User> GetById(string id)
    {
        try
        {
            await using var context = new ApplicationContext();
            return await context.UsersSet.Select(u => new User()
            {
                Id = u.Id,
                CreatedOn = u.CreatedOn,
                Username = u.Username,
                Image = u.Image,
                IsArtist = u.IsArtist,
                Email = u.Email,
            }).FirstOrDefaultAsync(u => u.Id == id);
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        await using var context = new ApplicationContext();

        return await context.UsersSet
            .ToListAsync();
    }

    public async void Add(User entity)
    {
        await using var context = new ApplicationContext();

        await context.UsersSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(User entity)
    {
        await using var context = new ApplicationContext();

        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task Delete(User entity)
    {
        await using var context = new ApplicationContext();

        context.UsersSet.Remove(entity);
        await context.SaveChangesAsync();
    }


    public async Task<IEnumerable<Post>> GetUserPosts(string id)
    {
        await using var context = new ApplicationContext();

        var a = await context.PostsSet
            .Select(p => new Post()
            {
                Likes = p.Likes.Select(l => new User()
                {
                    Id = l.Id
                }).ToList(),
                User = new User()
                {
                    Id = p.UserId,
                    Username = p.User.Username,
                },
                Id = p.Id,
                Text = p.Text,
                CreatedOn = p.CreatedOn,
                UserId = p.UserId,
                Playlist = p.Playlist
            }).AsNoTracking()
            .Where(p => p.UserId == id)
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();
        return a;
    }

    public async Task<IEnumerable<Playlist>> GetPlaylists(string id)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Playlists)
            .SingleAsync(u => u.Id == id);
        return user.Playlists;
    }
    
    public async Task<IEnumerable<Playlist>> GetOwnedPlaylists(string id)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Playlists)
            .SingleAsync(u => u.Id == id);
        return user.Playlists;
    }

    public async Task<IEnumerable<User>> GetFollowers(string id)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Follows)
            .SingleAsync(u => u.Id == id);
        return user.Follows;
    }

    public async Task<IEnumerable<User>> GetSubscribers(string id)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Subscribers)
            .SingleAsync(u => u.Id == id);
        return user.Subscribers;
    }

    public async Task<int> GetFollowersCount(string id)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Follows)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? 0 : user.Follows.Count;
    }

    public async Task<int> GetSubscribersCount(string id)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Subscribers)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? 0 : user.Subscribers.Count;
    }

    public async Task<bool> GetIsFollowed(string userid, string accountId)
    {
        await using var context = new ApplicationContext();

        var user = await context.UsersSet
            .Include(u => u.Subscribers)
            .FirstOrDefaultAsync(u => u.Id == userid);

        if (user == null || user.Subscribers.Count <= 0) return false;
        return user.Subscribers.Any(a => a.Id == accountId);
    }

    public async Task Follow(string user1, string user2)
    {
        await using var context = new ApplicationContext();

        var u1 = await context.UsersSet
            .Include(u => u.Subscribers)
            .SingleAsync(u => u.Id == user1);
        
        var u2 = await context.UsersSet
            .Include(u => u.Follows)
            .SingleAsync(u => u.Id == user2);

        
        if (u1.Subscribers.Contains(u2))
        {
            u1.Subscribers.Remove(u2);
            //u2.Follows.Remove(u1);
        }
        else
        {
            u1.Subscribers.Add(u2);
            //u2.Follows.Add(u1);
        }

        await context.SaveChangesAsync();
    }

    public async Task<User> GetRandomByTrackId(string trackId)
    {
        await using var context = new ApplicationContext();
        return await context.TrackLikes
            .Where(x => x.TrackId == trackId)
            .OrderBy(x => Guid.NewGuid())
            .Include(x => x.User)
            .Select(x => x.User)
            .FirstOrDefaultAsync();
    }
}