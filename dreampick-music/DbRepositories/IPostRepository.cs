using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IPostRepository : IRepositoryAsync<Post>
{
    public Task<int> GetLikesCount(string id);
    public Task<IEnumerable<User>> GetLikes(string id);
    Task AddLike(string id, User user);


    
    Task<bool> GetIsLiked(string postid, string accountId);

}