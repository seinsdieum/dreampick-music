using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IAccountRepository
{
    
    Task<(bool, User)> GetAuthenticatedUser(string id, string hashedPassword);

    Task<bool> AddUserAccount(User user, string hashedPassword);

    Task<bool> Disable(string id, string hashedPassword);

    Task<bool> Exists(string name);

    Task<bool> GetAuthentication(string id, string hashedPassword);
    Task Update(Account account);

}