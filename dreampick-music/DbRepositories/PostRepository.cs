using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;
using ApplicationContext = dreampick_music.DbContexts.ApplicationContext;

namespace dreampick_music.DbRepositories;

public class PostRepository : IPostRepository
{
    public async Task<Post> GetById(string id)
    {
        throw new NotImplementedException();
        /*return await context.Posts
            .Include(p => p.Likes.Count)
            .Include(p => p.Playlist)
            .Include(p => p.User)
            .SingleAsync(p => p.Id == id);*/
    }

    public async Task<IEnumerable<Post>> GetAll()
    {
        await using var context = new ApplicationContext();
        try
        {
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
                        Image = p.User.Image,
                    },
                    Id = p.Id,
                    Text = p.Text,
                    CreatedOn = p.CreatedOn,
                    UserId = p.UserId,
                    Playlist = p.Playlist
                }).AsNoTracking()
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
            return a;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
            return new List<Post>();
        }
    }

    public void Add(Post entity)
    {
        using var context = new ApplicationContext();

        var pl =  context.PlaylistsSet.Find(entity.Playlist.Id);

        var post = new Post()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Text = entity.Text,
            CreatedOn = entity.CreatedOn,
            Playlist = pl
        };

        try
        {
            context.PostsSet.Add(entity);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
        }
    }

    public async Task Update(Post entity)
    {
        await using var context = new ApplicationContext();

        try
        {
            var a = await context.PostsSet.FindAsync(entity.Id);
            if (a == null) return;
            context.Entry(a).CurrentValues.SetValues(entity);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
        }
    }

    public async Task Delete(Post entity)
    {
        throw new System.NotImplementedException();
    }

    public async Task<int> GetLikesCount(string id)
    {
        await using var context = new ApplicationContext();
        var post = await context.PostsSet.Include(post => post.Likes).AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);


        return post != null ? post.Likes.Count : 0;
    }

    public async Task<IEnumerable<User>> GetLikes(string id)
    {
        await using var context = new ApplicationContext();

        var post = await context.PostsSet.Where(p => p.Id == id).AsNoTracking().Select(p => p.Likes).ToListAsync();

        return post.ElementAt(0);
    }

    public async Task<bool> AddLike(string postId, string accountId)
    {
        await using var context = new ApplicationContext();

        try
        {
            var p = await context.PostsSet.FirstOrDefaultAsync(p => p.Id == postId);
            var a = await context.UsersSet.FirstOrDefaultAsync(u => u.Id == accountId);

            if (p == null || a == null) return false;

            a.LikedPosts.Add(p);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
            return false;
        }
    }

    public async Task<bool> RemoveLike(string postId, string accountId)
    {
        await using var context = new ApplicationContext();

        try
        {
            var a = await context.UsersSet
                .Include(u => u.LikedPosts).FirstOrDefaultAsync(u => u.Id == accountId);
            var p = await context.PostsSet.FindAsync(postId);

            if (a == null || p == null) return false;
            
            a.LikedPosts.Remove(p);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
            return false;
        }
    }


    public async Task<bool> GetIsLiked(string postId, string accountId)
    {
        await using var context = new ApplicationContext();

        return await context.PostsSet
            .AnyAsync(p => p.Id == postId && p.Likes.Select(l => l.Id).Contains(accountId));
    }
}