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

        public async Task<ICollection<OcurrenceRecord>> GetOcurrencesRecord(int id) =>
            await _bankContext.OccurrenceRecords.Where(o => o.AccountId == id).ToListAsync();

        public async Task<ICollection<OcurrenceRecord>> Statement(int id) =>
            await GetOcurrencesRecord(id);

        public async Task<Account>? FindAccountById(int id) =>  
            await _bankContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        

        public async Task<Account>? AccountValid(int id, int numberAccount)
        {
            return await _bankContext.Accounts.Where(a => a.Id == id)
                                              .Where(a => a.NumberAccount == numberAccount)
                                              .FirstOrDefaultAsync();
        }

        public async Task<ICollection<OcurrenceRecord>> FilterYear(int year, Account account)
        {

            var filteredYear = await _bankContext.OccurrenceRecords.Where(o => o.CreatedAt.Year == year)
                                                                  .Where(o => o.AccountId == account.Id)
                                                                  .ToListAsync();
            return filteredYear;

        }

        public ICollection<dynamic> FilterMonth(ICollection<OcurrenceRecord> listOccurrenceRecords)
        {
            var query = from o in listOccurrenceRecords
                        group o by o.CreatedAt.Month into ocr
                        select new
                        {
                            Id = ocr.Key,
                            Credits = ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Credit ? o.Amount : 0),
                            Debits = ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Debit ? o.Amount : 0),
                            Balance = (ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Credit ? o.Amount : 0)
                                      - ocr.Sum(o => o.TypeRecord == Models.Enums.TypeRecord.Debit ? o.Amount : 0))
                        };

            List<dynamic> filteredMonth= new() { query };
            return filteredMonth;
        }    
    }
}
