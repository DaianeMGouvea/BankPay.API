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

        public async Task<ICollection<OcurrenceRecord>> GetOcurrencesRecord(int id) =>
            await _bankContext.OcurrenceRecords.Where(o => o.AccountId == id).ToListAsync();

        public async Task<ICollection<OcurrenceRecord>> Statement(int id) =>
            await GetOcurrencesRecord(id);

        public async Task<ICollection<OcurrenceRecord>> OcurrencesRecordYear(int year)
        {
            var filteredYear = await _bankContext.OcurrenceRecords.Where(o => o.CreatedAt.Year == year).ToListAsync();
            return filteredYear; 
        }

        public async Task<Account>? FindAccountById(int id) =>  
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        

        public async Task<Account>? AccountValid(int id, int numberAccount)
        {
            return await _bankContext.Accounts.Where(a => a.Id == id)
                                              .Where(a => a.NumberAccount == numberAccount)
                                              .FirstOrDefaultAsync();
        }

       
    }
}
