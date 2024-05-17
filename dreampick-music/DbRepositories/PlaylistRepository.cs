using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;
using ApplicationContext = dreampick_music.DbContexts.ApplicationContext;

namespace dreampick_music.DbRepositories;

public class PlaylistRepository : IPlaylistRepository
{

    public async Task<Playlist> GetById(string id)
    {
        await using var context = new ApplicationContext();
        return await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.Likes)
            .Include(p => p.Tracks)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Playlist>> GetAll()
    {
        await using var context = new ApplicationContext();

        return await context.PlaylistsSet
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedOn)
                .Include(p => p.User)
                .ToListAsync()
            ;
    }
    

    public async Task<IEnumerable<Playlist>> GetByUserId(string userId)
    {
        // auto m2m
        await using var context = new ApplicationContext();/*
        var u = await context.UsersSet
            .AsNoTracking().Include(u => u.Playlists).FirstOrDefaultAsync(u => u.Id == userId);
        return u.Playlists;*/
        
        // pre m2m
        return await context.PlaylistLikes
            .OrderByDescending(p => p.CreatedOn)
            .Where(p => p.UserId == userId)
            .Select(p => p.Playlist)
            .ToListAsync();
    }

    public async Task<IEnumerable<Playlist>> GetSome(int n = 5)
    {
        await using var context = new ApplicationContext();

        return await context.PlaylistsSet
                .AsNoTracking()
            .Include(p => p.User)
            .Take(n)
                .Where(p => !p.IsUserPlaylist)
            .ToListAsync()
            ;
    }

    public async void Add(Playlist entity)
    {
        
        await using var context = new ApplicationContext();
        
        
        
        await context.PlaylistsSet.AddAsync(entity);

        await context.SaveChangesAsync();
    }

    public async void AddCustom(Playlist entity, ICollection<string> tracksId, string userId)
    {
        await using var context = new ApplicationContext();

        await context.PlaylistsSet.AddAsync(entity);

        foreach (var id in tracksId)
        {
            var a = await context.TracksSet.FirstOrDefaultAsync(t => t.Id == id);
            if(a != null) entity.UserAddedTracks.Add(a);
        }

        var user = await context.UsersSet.FirstOrDefaultAsync(u => u.Id == userId);
        entity.User = user;

        await context.SaveChangesAsync();

    }

    public async Task Update(Playlist entity)
    {
        try
        {

            await using var context = new ApplicationContext();

            
            var t = await context.TracksSet.Where(t => t.PlaylistId == entity.Id).ToListAsync();
            
            if(t.Count != 0) context.TracksSet.RemoveRange(t);
            /*await context.TracksSet.AddRangeAsync(entity.Tracks);
            context.Entry(entity).Collection(p => p.Tracks).IsModified = true;
            */
            
            /*await context.SaveChangesAsync();*/

            var e = await context.PlaylistsSet.FirstOrDefaultAsync(p => p.Id == entity.Id);
            

            e.Name = entity.Name;
            e.Description = entity.Description;
            e.Image = entity.Image;
            e.Genre = entity.Genre;
            e.Tracks = entity.Tracks;
            
            /*context.Entry(entity).Property(p => p.Name).IsModified = true;
            context.Entry(entity).Property(p => p.Description).IsModified = true;
            context.Entry(entity).Property(p => p.Image).IsModified = true;
            context.Entry(entity).Property(p => p.Genre).IsModified = true;*/

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
    }
    
    
    public async Task UpdateCustom(Playlist entity, List<string> tracksId)
    {
        try
        {

            await using var context = new ApplicationContext();
            

            var e = await context.PlaylistsSet.Include(p => p.UserAddedTracks).FirstOrDefaultAsync(p => p.Id == entity.Id);
            
            e.UserAddedTracks.RemoveRange(0, e.UserAddedTracks.Count);
            
            foreach (var id in tracksId)
            {
                var a = await context.TracksSet.FirstOrDefaultAsync(t => t.Id == id);
                if(a != null) e.UserAddedTracks.Add(a);
            }

            e.Name = entity.Name;
            e.Description = entity.Description;
            e.Image = entity.Image;

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
    }

    public async Task Delete(Playlist entity)
    {
        
        await using var context = new ApplicationContext();

        context.PlaylistsSet.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<Playlist> GetWithTracksById(string id)
    {
        await using var context = new ApplicationContext();
        return await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.Tracks)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> GetIsLiked(string id, string accountId)
    {
        await using var context = new ApplicationContext();
        
        return await context.PlaylistsSet.AsNoTracking().AnyAsync(p => p.Id == id && p.Likes.Select(l => l.Id).Contains(accountId));
    }

    public async Task AddLike(string albumId, string userId)
    {
        await using var context = new ApplicationContext();

        var a = await context.PlaylistsSet.FirstOrDefaultAsync(p => p.Id == albumId);

        var u = await context.UsersSet.FirstOrDefaultAsync(u => u.Id == userId);
        if (u != null && a != null)
        {
            u.Playlists.Add(a);
            await context.SaveChangesAsync();
        }
        
    }

    public async Task RemoveLike(string albumId, string userId)
    {
        await using var context = new ApplicationContext();

        var a = await context.PlaylistsSet.FirstOrDefaultAsync(p => p.Id == albumId);

        var u = await context.UsersSet.Include(u => u.Playlists).FirstOrDefaultAsync(u => u.Id == userId);
        if (a != null && u != null)
        {
            u.Playlists.Remove(a);
            await context.SaveChangesAsync();
        }
        
    }


    public async Task<int> GetLikesCount(string id)
    {
        
        await using var context = new ApplicationContext();

        // auto m2m
        
        /*var a = await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.Likes.Count)
            .SingleAsync(p => p.Id == id);
        return a.Likes.Count;*/
        return await context.PlaylistLikes.Where(p => p.PlaylistId == id).CountAsync();
    }

    public async Task<IEnumerable<Track>> GetPlaylistTracks(string playlistId)
    {
        
        await using var context = new ApplicationContext();

        var a = await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.Tracks)
            .SingleAsync(p => p.Id == playlistId);

        return a.Tracks;
    }

    public async Task<IEnumerable<Playlist>> GetLast(string id, int n = 5)
    {
        
        await using var context = new ApplicationContext();

        var a = await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => !p.IsUserPlaylist)
            .ToListAsync();

        return a.GetRange(0, n);
    }

    public async Task<IEnumerable<Playlist>> GetAllByGenre(Genre genre)
    {

        await using var context = new ApplicationContext();

        return await context.PlaylistsSet
            .AsNoTracking()
            .OrderByDescending(p => p.Likes.Count)
            .Include(p => p.User)
            .Where(p => p.Genre == genre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Playlist>> GetAllByArtist(string artistId)
    {
        await using var context = new ApplicationContext();

        return await context.PlaylistsSet.Include(p => p.User)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOn)
            .Where(p => p.User.Id == artistId && !p.IsUserPlaylist)
            .ToListAsync();
    }

    public async Task<IEnumerable<Playlist>> GetAllCustomByUserId(string userId)
    {
        await using var context = new ApplicationContext();
        return await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.UserAddedTracks)
            .Include(p => p.Likes)
            .Include(p => p.User)
            .Where(p => p.Likes.Any(u => u.Id == userId))
            .ToListAsync()
            ;
    }

    public async Task<IEnumerable<Playlist>> GetOwnCustomByUserId(string userId)
    {
        await using var context = new ApplicationContext();
        return await context.PlaylistsSet
                .Include(p => p.UserAddedTracks)
                .ThenInclude(t => t.Playlist)
                .ThenInclude(t => t.User)
                .Include(p => p.Likes)
                .Include(p => p.User)
                .Where(p => p.User.Id == userId && p.IsUserPlaylist)
                .ToListAsync()
            ;
    }
    
    public async Task<Playlist> GetCustomById(string id)
    {
        await using var context = new ApplicationContext();
        return await context.PlaylistsSet
            .AsNoTracking()
            .Include(p => p.UserAddedTracks)
            .ThenInclude(t => t.Playlist)
            .ThenInclude(t => t.User)
            .Include(p => p.Likes)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}