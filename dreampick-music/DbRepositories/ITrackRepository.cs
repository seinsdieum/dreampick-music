using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface ITrackRepository : IRepositoryAsync<Track>
{
    Task<int> GetLikesCount(string id);

    Task<bool> GetIsLiked(string id, string userId);
    
    Task AddLike(string id, User user);

}