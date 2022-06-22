using BankPay.API.Data;
using BankPay.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankPay.API.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BankPayApiContext _bankContext;

        public UsersRepository(BankPayApiContext bankContext)
        {
            _bankContext = bankContext; 
        }

        public async Task<ICollection<User>> AddUser(User user)
        {
            _bankContext.Add(user);
            _bankContext.SaveChangesAsync();
            return await _bankContext.Users.ToListAsync();
        }

        public async void Update(User user)
        {
            _bankContext.Users.Update(user);
            _bankContext.SaveChanges();

            await _bankContext.Users.Where(u => u.Id == user.Id)
                              .FirstOrDefaultAsync(); 
        }

        public void Delete(User user)
        {
            throw new NotImplementedException();
        }    

        public async Task<User> FindBy(int key)
        {
            return await _bankContext.Users.Include(u => u.Account)
                                           .FirstOrDefaultAsync(u => u.Id == key);
        }

        public async Task<ICollection<User>> GetUsers()
        {
            return await _bankContext.Users.Include(u => u.Account)
                                           .ToListAsync();
        }

    }
}
