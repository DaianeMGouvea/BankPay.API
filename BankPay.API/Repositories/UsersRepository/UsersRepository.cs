using BankPay.API.Data;
using BankPay.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankPay.API.Repositories.UsersRepository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BankPayApiContext _bankContext;

        public UsersRepository(BankPayApiContext bankContext)
        {
            _bankContext = bankContext;
        }

        public async Task<bool> UserExist(User user)
        {        
            var a = await _bankContext.Users.Where(u => u.Name == user.Name)
                                            .Where(u => u.Account.Id == user.Account.Id).ToListAsync();
            return a.Any();
        }

        public async Task<ICollection<User>> AddUser(User user)
        {
            _bankContext.Add(user);
            _bankContext.SaveChangesAsync();
            return await _bankContext.Users.ToListAsync();
        }

        public async Task<int> Update(User user)
        {
            _bankContext.Users.Update(user);
            return await _bankContext.SaveChangesAsync();
        }

        public async Task<int> Delete(User user)
        {
            _bankContext.Users.Remove(user);
            return await _bankContext.SaveChangesAsync();
        }

        public async Task<User>? FindBy(int key)
        {
            return await _bankContext.Users.Include(u => u.Account)
                                           .FirstOrDefaultAsync(u => u.Id == key);
        }

        public async Task<ICollection<User>>? GetUsers()
        {
            return await _bankContext.Users.Include(u => u.Account)
                                           .ToListAsync();
        }


    }
}
