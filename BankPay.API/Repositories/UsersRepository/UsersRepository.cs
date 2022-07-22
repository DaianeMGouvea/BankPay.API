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

        public async Task<bool> UserExist(String name)
        {        
            var userExist = await _bankContext.Users.Where(u => u.Name == name).ToListAsync();
            return userExist.Any();
        }

        public async Task<int> AddUser(User user)
        {
            _bankContext.Add(user);         
            return await _bankContext.SaveChangesAsync();
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
