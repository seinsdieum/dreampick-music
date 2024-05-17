using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface ITrackRepository : IRepositoryAsync<Track>
{
    Task<int> GetLikesCount(string id);

    Task<bool> GetIsLiked(string id, string userId);
    
    Task AddLike(string trackId, string userId);
    Task RemoveLike(string trackId, string userId);

    Task<ICollection<Track>> GetByUserId(string userId);
    //Task<ICollection<Track>> GetByCustomPlaylistId(string );
    
    Task<DbContexts.Track> GetRandomByUserId(string userId);

    
}