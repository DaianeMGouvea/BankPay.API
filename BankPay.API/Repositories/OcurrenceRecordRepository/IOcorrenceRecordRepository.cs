using BankPay.API.Models;

namespace BankPay.API.Repositories.OccurrenceRecordRepository
{
    public interface IOccurrenceRecordRepository
    {
        Task<IEnumerable<OcurrenceRecord>> GetOcurrencesRecord(int id);
        Task<IEnumerable<OcurrenceRecord>> Statement(int id);
        Task<Account>? FindAccountById(int id);
        Task<Account>? AccountValid(int numberAccount);
        Task<IEnumerable<OcurrenceRecord>> FilterYear(int year, Account account);
        IEnumerable<dynamic> FilterMonth(IEnumerable<OcurrenceRecord> listOccurrenceRecords);
    }
}