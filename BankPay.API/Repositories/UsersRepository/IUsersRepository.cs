using BankPay.API.Models;

namespace BankPay.API.Repositories.UsersRepository
{
    public interface IUsersRepository
    {
        Task<bool> UserExist(String user);
        Task<int> AddUser(User user);
        Task<int> Update(User user);
        Task<int> Delete(User id);
        Task<ICollection<User>>? GetUsers();
        Task<User>? FindBy(int key);
    }
}
