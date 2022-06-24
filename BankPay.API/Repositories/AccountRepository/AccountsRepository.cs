using BankPay.API.Data;
using BankPay.API.Models;
using BankPay.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankPay.API.Repositories.AccountRepository
{
    public class AccountsRepository : IAccountsRepository
    {

        private readonly BankPayApiContext _bankContext;

        public AccountsRepository(BankPayApiContext bankContext)
        {
            _bankContext = bankContext;
        }

        public async Task<int> AddCredit(Account account, double amount)
        {
            account.Balance += amount;

            _bankContext.OcurrenceRecords.Add(new OcurrenceRecord(TypeRecord.Credit, amount));
            return await _bankContext.SaveChangesAsync();
        }

        public async Task<int> Withdraw(Account account, double amount)
        {
            account.Balance -= amount;

            _bankContext.OcurrenceRecords.Add(new OcurrenceRecord(TypeRecord.Debit, amount));
            return await _bankContext.SaveChangesAsync();
        }

        public async Task<ICollection<Account>>? GetAccounts() =>
            await _bankContext.Accounts.ToListAsync();

        public async Task<Account>? FindBy(int id) =>
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Account>? CheckNumberAccount(int numberAccount) =>
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.NumberAccount == numberAccount);



    }
}
