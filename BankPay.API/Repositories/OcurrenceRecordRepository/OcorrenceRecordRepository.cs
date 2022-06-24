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
        public async Task<ICollection<OcurrenceRecord>> GetOcurrenceRecords() =>
            await _bankContext.OcurrenceRecords.ToListAsync();

        public async Task<ICollection<OcurrenceRecord>> Statement() =>
            await GetOcurrenceRecords();


        public async Task<ICollection<OcurrenceRecord>> OcurrenceRecordYear(int year)
        {
            return null;
        }
            

    }
}
