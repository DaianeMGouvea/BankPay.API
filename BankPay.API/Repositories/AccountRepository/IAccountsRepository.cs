using BankPay.API.Models;

namespace BankPay.API.Repositories.AccountRepository
{
    public interface IAccountsRepository
    {
        Task<int> AddCredit(Account account, double amount);
        Task<int> Withdraw(Account account, double amount);
        Task<Account>? FindBy(int id);
        Task<ICollection<Account>>? GetAccounts();
        Task<Account>? CheckNumberAccount(int numberAccount);

    }
}
