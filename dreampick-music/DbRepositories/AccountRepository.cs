using System.Collections.Generic;
using System.Threading.Tasks;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace dreampick_music.DbRepositories;

public class AccountRepository : IAccountRepository
{

    private ApplicationContext _context = new ApplicationContext();

    public async Task<bool> GetTryAuth(string id, string hashedPassword)
    {
        var account = await _context.Accounts.FindAsync(id);
        return !account.disabled && account.HashedPassword == hashedPassword;
    }

    public async Task<bool> GetTryRegister(User user, string hashedPassword)
    {
        var account = new Account()
        {
            disabled = false,
            HashedPassword = hashedPassword,
            Id = user.Id
        };
        var a = _context.Users.AddAsync(user);
        var b = _context.Accounts.AddAsync(account);
        await a;
        await b;
        var c = _context.SaveChangesAsync();
        await c;

        return a.IsCompletedSuccessfully && b.IsCompletedSuccessfully && c.IsCompletedSuccessfully;
    }
    
    
    

    public async Task<bool> Disable(string id, string hashedPassword)
    {
        var a = await _context.Accounts.FindAsync(id);
        if (a is not null) a.disabled = true;
        _context.Accounts.Update(a);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Exists(string name)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == name);
    }
}