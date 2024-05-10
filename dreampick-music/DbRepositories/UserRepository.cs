
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbRepositories;

public class UserRepository : 
    IUserRepository

{
    private readonly ApplicationContext context = new ApplicationContext();
    public async Task<User> GetById(string id)
    {
        return await context.Users
            .Include(u => u.Subscribers.Count)
            .Include(u => u.Follows.Count)
            .Include(u => u.Posts)
            .SingleAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await context.Users
            .ToListAsync();
    }

    public async Task Add(User entity)
    {
        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(User entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task Delete(User entity)
    {
        context.Users.Remove(entity);
        await context.SaveChangesAsync();
    }


    public async Task<IEnumerable<Post>> GetUserPosts(string id)
    {
        var user = await context.Users
            .Include(u => u.Posts)
            .ThenInclude(p => p.Playlist)
            .ThenInclude(p => p.User)
            .SingleAsync(u => u.Id == id);
        return user.Posts;
    }

    public async Task<IEnumerable<Playlist>> GetPlaylists(string id)
    {
        var user = await context.Users
            .Include(u => u.Playlists)
            .SingleAsync(u => u.Id == id);
        return user.Playlists;
    }

    public async Task<IEnumerable<User>> GetFollowers(string id)
    {
        var user = await context.Users
            .Include(u => u.Follows)
            .SingleAsync(u => u.Id == id);
        return user.Follows;
    }

    public async Task<IEnumerable<User>> GetSubscribers(string id)
    {
        var user = await context.Users
            .Include(u => u.Subscribers)
            .SingleAsync(u => u.Id == id);
        return user.Subscribers;
    }

    public async Task<int> GetFollowersCount(string id)
    {
        var user = await context.Users
            .Include(u => u.Follows)
            .SingleAsync(u => u.Id == id);
        return user.Follows.Count;
    }

    public async Task<int> GetSubscribersCount(string id)
    {
        var user = await context.Users
            .Include(u => u.Subscribers)
            .SingleAsync(u => u.Id == id);
        return user.Subscribers.Count;
    }

    public async Task<bool> GetIsFollowed(string userid, string accountId)
    {
        var user = await context.Users
            .Include(u => u.Subscribers)
            .SingleAsync(u => u.Id == userid);
        return user.Subscribers.Any(a => a.Id == accountId);
    }

    public async Task Follow(string user1, string user2)
    {
        var u1 = await context.Users
            .Include(u => u.Subscribers)
            .SingleAsync(u => u.Id == user1);
        
        var u2 = await context.Users
            .Include(u => u.Follows)
            .SingleAsync(u => u.Id == user2);

        
        if (u1.Subscribers.Contains(u2))
        {
            u1.Subscribers.Remove(u2);
            u2.Follows.Remove(u1);
        }
        else
        {
            u1.Subscribers.Add(u2);
            u2.Follows.Add(u1);
        }

        await context.SaveChangesAsync();
    }
}