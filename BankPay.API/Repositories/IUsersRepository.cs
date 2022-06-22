using BankPay.API.Models;

namespace BankPay.API.Repositories
{
    public interface IUsersRepository
    {
        Task<ICollection<User>> AddUser(User user);
        void Update(User user);
        void Delete(User user);
        Task<ICollection<User>> GetUsers();
        Task<User> FindBy(int key);
    }
}
