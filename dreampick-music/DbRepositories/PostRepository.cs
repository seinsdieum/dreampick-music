using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbRepositories;

public class PostRepository : IPostRepository
{

    private ApplicationContext _context = new();
    public async Task<Post> GetById(string id)
    {
        throw new NotImplementedException();
        /*return await _context.Posts
            .Include(p => p.Likes.Count)
            .Include(p => p.Playlist)
            .Include(p => p.User)
            .SingleAsync(p => p.Id == id);*/
    }

    public async Task<IEnumerable<Post>> GetAll()
    {
        return await _context.Posts
            .Include(p => p.Likes.Count)
            .Include(p => p.Playlist)
            .Include(p => p.User)
            .ToListAsync();
    }

    public async Task Add(Post entity)
    {
        await _context.Posts.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Post entity)
    {
        throw new System.NotImplementedException();
    }

    public async Task Delete(Post entity)
    {
        throw new System.NotImplementedException();
    }

    public async Task<int> GetLikesCount(string id)
    {
        var post = await _context.Posts
            .Include(p => p.Likes.Count)
            .SingleAsync(p => p.Id == id);

        return post.Likes.Count;
    }

    public async Task<IEnumerable<User>> GetLikes(string id)
    {
        var post = await _context.Posts
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == id);

        return post.Likes;
    }

    public async Task AddLike(string id, User user)
    {
        var posts = await _context.Posts
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == id);
        
        posts.Likes.Add(user);

        await _context.SaveChangesAsync();
    }


    public async Task<bool> GetIsLiked(string postId, string accountId)
    {
        var post = await _context.Posts
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == postId);
        return post.Likes.Any(a => a.Id == accountId);
    }
    
}