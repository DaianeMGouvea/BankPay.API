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
            var record = new OcurrenceRecord(TypeRecord.Credit, amount, account.Id);
            _bankContext.OccurrenceRecords.Add( record );
            return await _bankContext.SaveChangesAsync();
        }

        public async Task<int> Withdraw(Account account, double amount)
        {
            account.Balance -= amount;

            var record = new OcurrenceRecord(TypeRecord.Debit, amount, account.Id);
            _bankContext.OccurrenceRecords.Add(record);
            return await _bankContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>>? GetAccounts() =>
            await _bankContext.Accounts.ToListAsync();

        public async Task<Account>? FindById(int id) =>
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Account>? FindByNumberAccount(int numberAccount)
        {
            return await _bankContext.Accounts.Where(a => a.NumberAccount == numberAccount)
                                              .FirstOrDefaultAsync();
        }
    }
}
