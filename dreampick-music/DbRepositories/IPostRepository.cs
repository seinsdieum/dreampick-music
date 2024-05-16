using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IPostRepository : IRepositoryAsync<Post>
{
    public Task<int> GetLikesCount(string id);
    public Task<IEnumerable<User>> GetLikes(string id);
    Task<bool> AddLike(string postId, string accountId);
    Task<bool> RemoveLike(string postId, string accountId);


    
    Task<bool> GetIsLiked(string postid, string accountId);

}