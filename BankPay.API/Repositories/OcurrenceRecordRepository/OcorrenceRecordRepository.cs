using BankPay.API.Data;
using BankPay.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankPay.API.Repositories.OcurrenceRecordRepository
{
    public class OcorrenceRecordRepository : IOcorrenceRecordRepository
    {
        private readonly BankPayApiContext _bankContext;

        public OcorrenceRecordRepository(BankPayApiContext bankContext)
        {
            _bankContext = bankContext;
        }

        public async Task<ICollection<OcurrenceRecord>> GetOcurrencesRecord() =>
            await _bankContext.OcurrenceRecords.ToListAsync();

        public async Task<ICollection<OcurrenceRecord>> Statement() =>
            await GetOcurrencesRecord();

        public async Task<ICollection<OcurrenceRecord>> OcurrencesRecordYear(int year)
        {
            var filteredYear = await _bankContext.OcurrenceRecords.Where(o => o.CreatedAt.Year == year).ToListAsync();
            return filteredYear; 
        }
        
        public async Task<Account>? FindByNumberAccount(int numberAccount) =>
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.NumberAccount == numberAccount);
    }
}
