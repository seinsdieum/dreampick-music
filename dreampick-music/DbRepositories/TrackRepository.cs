using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbRepositories;

public class TrackRepository : ITrackRepository
{

    private ApplicationContext context = new ApplicationContext();
    public async Task<Track> GetById(string id)
    {
        return await context.Tracks
            .Include(t => t.Playlist)
            .Include(t => t.Likes)
            .SingleAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Track>> GetAll()
    {
        return await context.Tracks
            .Include(t => t.Playlist)
            .ToListAsync();
    }

    public async Task Add(Track entity)
    {
        await context.Tracks.AddAsync(entity);

        await context.SaveChangesAsync();
    }

    public async Task Update(Track entity)
    {
        context.Entry(entity).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async Task Delete(Track entity)
    {
        context.Tracks.Remove(entity);

        await context.SaveChangesAsync();
    }

    public async Task<int> GetLikesCount(string id)
    {
        var a = await context.Tracks
            .Include(t => t.Likes.Count)
            .SingleAsync(t => t.Id == id);

        return a.Likes.Count;
    }

    public async Task<bool> GetIsLiked(string id, string userId)
    {
        var a = await context.Tracks
            .Include(t => t.Likes)
            .SingleAsync(t => t.Id == id);

        return a.Likes.Any(u => u.Id == userId);
    }

    public async Task AddLike(string id, User user)
    {
        var a = await context.Tracks
            .Include(t => t.Likes)
            .SingleAsync(t => t.Id == id);
        a.Likes.Add(user);

        await context.SaveChangesAsync();
    }
}