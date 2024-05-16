using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IPlaylistRepository : IRepositoryAsync<Playlist>
{
    Task<Playlist> GetWithTracksById(string id);
    Task<bool> GetIsLiked(string id, string accountId);

    Task AddLike(string albumId, string userId);
    Task RemoveLike(string albumId, string userId);


    Task<int> GetLikesCount(string id);

    Task<IEnumerable<Track>> GetPlaylistTracks(string playlistId);
    Task<IEnumerable<Playlist>> GetLast(string id, int n = 5);

    Task<IEnumerable<Playlist>> GetAllByGenre(Genre genre);

    Task<IEnumerable<Playlist>> GetAllByArtist(string artistId);
    Task<IEnumerable<Playlist>> GetByUserId(string userId);
    

    public Task<IEnumerable<Playlist>> GetSome(int n = 0);

}