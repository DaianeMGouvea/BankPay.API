using BankPay.API.Data;
using BankPay.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankPay.API.Repositories.OccurrenceRecordRepository
{
    public class OccurrenceRecordRepository : IOccurrenceRecordRepository
    {
        private readonly BankPayApiContext _bankContext;

        public OccurrenceRecordRepository(BankPayApiContext bankContext)
        {
            _bankContext = bankContext;
        }

        public async Task<IEnumerable<OcurrenceRecord>> GetOcurrenceById(int id) =>
            await _bankContext.OccurrenceRecords.Where(o => o.AccountId == id).ToListAsync();

        public async Task<IEnumerable<OcurrenceRecord>> GetOcurrencesRecord() =>
            await _bankContext.OccurrenceRecords.ToListAsync();

        public async Task<IEnumerable<OcurrenceRecord>> Statement(int id) =>
            await GetOcurrenceById(id);

        public async Task<Account>? FindAccountById(int id) =>  
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        

        public async Task<Account>? AccountValid(int numberAccount)
        {
            return await _bankContext.Accounts.FirstOrDefaultAsync(a => a.NumberAccount == numberAccount);
        }

        public async Task<IEnumerable<OcurrenceRecord>> FilterYear(int year, Account account)
        {

            var filteredYear = await _bankContext.OccurrenceRecords.Where(o => o.CreatedAt.Year == year)
                                                                  .Where(o => o.AccountId == account.Id)
                                                                  .ToListAsync();
            return filteredYear;

        }

        public IEnumerable<dynamic> FilterMonth(IEnumerable<OcurrenceRecord> listOccurrenceRecords)
        {
            var query = from o in listOccurrenceRecords
                        group o by o.CreatedAt.Month into ocr
                        select new
                        {
                            Month = ocr.Key,
                            Credits = ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Credit ? o.Amount : 0),
                            Debits  = ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Debit ? o.Amount : 0),
                            Balance = (ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Credit ? o.Amount : 0)
                                      - ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Debit ? o.Amount : 0))
                        };

            return query;
        }    
    }
}
