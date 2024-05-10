using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IPlaylistRepository : IRepositoryAsync<Playlist>
{
    Task<bool> GetIsLiked(string id, string accountId);

    Task AddLike(string id, User user);


    Task<int> GetLikesCount(string id);

    Task<IEnumerable<Track>> GetPlaylistTracks(string id);
    Task<IEnumerable<Playlist>> GetLast(string id, int n = 5);

    Task<IEnumerable<Playlist>> GetByGenre(Genre genre);

}