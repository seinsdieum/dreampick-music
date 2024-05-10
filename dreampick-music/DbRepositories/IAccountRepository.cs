using System.Threading.Tasks;
using dreampick_music.DbContexts;

namespace dreampick_music.DbRepositories;

public interface IAccountRepository
{
    
    Task<bool> GetTryAuth(string id, string hashedPassword);

    Task<bool> GetTryRegister(User user, string hashedPassword);

    Task<bool> Disable(string id, string hashedPassword);

    Task<bool> Exists(string name);

}