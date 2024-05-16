
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IUserRepository : IRepositoryAsync<User>
{
    Task<IEnumerable<DbContexts.Post>> GetUserPosts(string id);
    Task<IEnumerable<DbContexts.Playlist>> GetPlaylists(string id);
    Task<IEnumerable<DbContexts.Playlist>> GetOwnedPlaylists(string id);
    Task<IEnumerable<DbContexts.User>> GetFollowers(string id);
    Task<IEnumerable<DbContexts.User>> GetSubscribers(string id);
    
    Task<int> GetFollowersCount(string id);
    Task<int> GetSubscribersCount(string id);

    Task<bool> GetIsFollowed(string userid, string accountId);
    
    Task Follow(string id1, string id);

    Task<DbContexts.User> GetRandomByTrackId(string trackId);


}