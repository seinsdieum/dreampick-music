using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbRepositories;

public class PlaylistRepository : IPlaylistRepository
{

    private ApplicationContext context = new ApplicationContext();
    public async Task<Playlist> GetById(string id)
    {
        return await context.Playlists
            .Include(p => p.Likes.Count)
            .Include(p => p.Tracks)
            .Include(p => p.User)

            .SingleAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Playlist>> GetAll()
    {
        return await context.Playlists
            .Include(p => p.User)
            .ToListAsync()
            ;
    }

    public async Task Add(Playlist entity)
    {
        await context.Playlists.AddAsync(entity);

        await context.SaveChangesAsync();
    }

    public async Task Update(Playlist entity)
    {
        context.Entry(entity).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async Task Delete(Playlist entity)
    {
        context.Playlists.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<bool> GetIsLiked(string id, string accountId)
    {
        var a = await context.Playlists
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == id);
        return a.Likes.Any(u => u.Id == id);
    }

    public async Task AddLike(string id, User user)
    {
        var a = await context.Playlists
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == id);
        
        a.Likes.Add(user);
    }

    public async Task<int> GetLikesCount(string id)
    {
        var a = await context.Playlists
            .Include(p => p.Likes.Count)
            .SingleAsync(p => p.Id == id);
        return a.Likes.Count;
    }

    public async Task<IEnumerable<Track>> GetPlaylistTracks(string id)
    {
        var a = await context.Playlists
            .Include(p => p.Tracks)
            .SingleAsync(p => p.Id == id);

        return a.Tracks;
    }

    public async Task<IEnumerable<Playlist>> GetLast(string id, int n = 5)
    {
        var a = await context.Playlists
            .Include(p => p.User)
            .ToListAsync();

        return a.GetRange(0, n);
    }

    public async Task<IEnumerable<Playlist>> GetByGenre(Genre genre)
    {

        return await context.Playlists
            .Include(p => p.User)
            .Where(p => p.Genre == genre)
            .ToListAsync();
    }
}