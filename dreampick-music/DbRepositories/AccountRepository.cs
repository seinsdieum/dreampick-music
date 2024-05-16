using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using dreampick_music.DbContexts;
using Microsoft.EntityFrameworkCore;
using ApplicationContext = dreampick_music.DbContexts.ApplicationContext;

namespace dreampick_music.DbRepositories;

public class AccountRepository : IAccountRepository
{
    
    
    public async Task<(bool, User)> GetAuthenticatedUser(string name, string hashedPassword)
    {
        await using var context = new ApplicationContext();
        try
        {
            var user = await context.UsersSet.SingleOrDefaultAsync(u => u.Username == name);

            if (user is null) return (false, null);
            var st = await context.AccountsSet.AnyAsync(a => a.Id == user.Id && a.HashedPassword == hashedPassword);

            if (st)
            {
                user = await context.UsersSet.Select(u => new User()
                {
                    
                    Id = u.Id,
                    Image = u.Image,
                    CreatedOn = u.CreatedOn,
                    LikedPosts = u.LikedPosts,
                    Username = u.Username,
                    IsArtist = u.IsArtist,
                    Email = u.Email,
                    
                    
                }).Where(u => u.Id == user.Id).AsNoTracking().SingleOrDefaultAsync();
            }
            
            return !st ? (false, null) : (true, user);
        }

        catch(Exception e)
        {
            MessageBox.Show($"{e.Message} {e.StackTrace}");
            return (false, null);
        }
    }
    

    public async Task<bool> AddUserAccount(User user, string hashedPassword)
    {
        await using var context = new ApplicationContext();

        try
        {
            var account = new Account()
            {
                disabled = false,
                HashedPassword = hashedPassword,
                Id = user.Id
            };
            await context.UsersSet.AddAsync(user);
            await context.AccountsSet.AddAsync(account);
            
            await context.SaveChangesAsync();
           

            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{MessageBox.Show(e.Message)} {e.StackTrace}");
            return false;
        }
    }
    
    
    

    public async Task<bool> Disable(string id, string hashedPassword)
    {
        await using var context = new ApplicationContext();

        var a = await context.AccountsSet.FindAsync(id);
        if (a is not null) a.disabled = true;
        context.AccountsSet.Update(a);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Exists(string name)
    {
        
        await using var context = new ApplicationContext();
        return await context.UsersSet
            .AnyAsync(u => u.Username == name);
    }

    public async Task<bool> GetAuthentication(string id, string hashedPassword)
    {
        await using var context = new ApplicationContext();

        var a = await context.AccountsSet.AnyAsync(u => u.Id == id && u.HashedPassword == hashedPassword);

        return a;

    }

    public async Task Update(Account account)
    {
        await using var context = new ApplicationContext();

        context.Entry(account).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }
}