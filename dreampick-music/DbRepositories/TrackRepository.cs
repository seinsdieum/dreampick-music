using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;
using ApplicationContext = dreampick_music.DbContexts.ApplicationContext;

namespace dreampick_music.DbRepositories;

public class TrackRepository : ITrackRepository
{

    public async Task<Track> GetById(string id)
    {

        await using var context = new ApplicationContext();
        
        return await context.TracksSet
            .Include(t => t.Playlist)
            .Include(t => t.Likes)
            .SingleAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Track>> GetAll()
    {
        
        
        await using var context = new ApplicationContext();

        return await context.TracksSet
            .Include(t => t.Playlist)
            .ToListAsync();
    }

    public async void Add(Track entity)
    {
        
        await using var context = new ApplicationContext();

        await context.TracksSet.AddAsync(entity);

        await context.SaveChangesAsync();
    }

    public async Task Update(Track entity)
    {
        
        await using var context = new ApplicationContext();

        context.Entry(entity).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async Task Delete(Track entity)
    {
        
        await using var context = new ApplicationContext();

        context.TracksSet.Remove(entity);

        await context.SaveChangesAsync();
    }

    public async Task<int> GetLikesCount(string id)
    {
        
        await using var context = new ApplicationContext();

        // auto m2m
        /*var a = await context.TracksSet
            .Include(t => t.Likes.Count)
            .SingleAsync(t => t.Id == id);

        return a.Likes.Count;*/
        
        //pre m2m
        return await context.TrackLikes.Where(t => t.UserId == id).CountAsync();
    }

    public async Task<bool> GetIsLiked(string id, string userId)
    {
        try
        {

            await using var context = new ApplicationContext();

            return await context.TracksSet.AnyAsync(p => p.Id == id && p.Likes.Select(l => l.Id).Contains(userId));
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
            return false;
        }
    }

    public async Task AddLike(string trackId, string userId)
    {
        try
        {
            await using var context = new ApplicationContext();

            var t = await context.TracksSet.FirstOrDefaultAsync(tt => tt.Id == trackId);
            var u = await context.UsersSet.FirstOrDefaultAsync(uu => uu.Id == userId);

            u.Tracks.Add(t);

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
    }

    public async Task RemoveLike(string trackId, string userId)
    {
        try
        {
            await using var context = new ApplicationContext();
            var t = await context.TracksSet.FirstOrDefaultAsync(tt => tt.Id == trackId);
            var u = await context.UsersSet.Include(uu => uu.Tracks).FirstOrDefaultAsync(uu => uu.Id == userId);
        
            u.Tracks.Remove(t);

            await context.SaveChangesAsync();

        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
    }

    public async Task<ICollection<Track>> GetByUserId(string userId)
    {
        await using var context = new ApplicationContext();

        // for auto m2m
        /*var a = await context.UsersSet
            .Include(u => u.Tracks)
            .ThenInclude(t => t.Playlist)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(u => u.Id == userId);
            return a.Tracks;
            */

        // for pre m2m
        try
        {
            return await
                context.TrackLikes
                    .Where(t => t.UserId == userId)
                    .Include(t => t.Track)
                    .ThenInclude(t => t.Playlist)
                    .ThenInclude(p => p.User)
                    .Select(t => t.Track)
                    .OrderByDescending(t => t.CreatedOn)
                    
                    
                    .ToListAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
            throw e;
        }


    }

    public async Task<Track> GetRandomByUserId(string userId)
    {
        await using var context = new ApplicationContext();

        return await context.TrackLikes
            .Where(x => x.UserId == userId)
            .OrderBy(x => Guid.NewGuid())
            .Include(x => x.Track)
            .ThenInclude(x => x.Playlist)
            .ThenInclude(x => x.User)
            .Select(x => x.Track)
            .FirstOrDefaultAsync();
    }


    /*public async Task<ICollection<Track>> GetRandomCollectionByUserId(string id)
    {
        await using var context = new ApplicationContext();
        
        return await context.TrackLikes
            .Include(x => x.User)
            .ThenInclude(x => x.Tracks)
            .Include(x => x.Track)
            .ThenInclude(x => x.Playlist)
            .ThenInclude(x => x.User)
    }*/
}