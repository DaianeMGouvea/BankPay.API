using BankPay.API.Models;

namespace BankPay.API.Repositories.OccurrenceRecordRepository
{
    public interface IOccurrenceRecordRepository
    {
        Task<ICollection<OcurrenceRecord>> GetOcurrencesRecord(int id);
        Task<ICollection<OcurrenceRecord>> Statement(int id);
        Task<Account>? FindAccountById(int id);
        Task<Account>? AccountValid(int numberAccount);
        Task<ICollection<OcurrenceRecord>> FilterYear(int year, Account account);
        ICollection<dynamic> FilterMonth(ICollection<OcurrenceRecord> listOccurrenceRecords);
    }
}