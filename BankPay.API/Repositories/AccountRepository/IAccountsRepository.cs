using BankPay.API.Models;

namespace BankPay.API.Repositories.AccountRepository
{
    public interface IAccountsRepository
    {
        Task<int> AddCredit(Account account, double amount);
        Task<int> Withdraw(Account account, double amount);
        Task<ICollection<Account>>? GetAccounts();
        Task<Account>? FindById(int id);
        Task<Account>? FindByNumberAccount(int numberAccount);

    }
}
