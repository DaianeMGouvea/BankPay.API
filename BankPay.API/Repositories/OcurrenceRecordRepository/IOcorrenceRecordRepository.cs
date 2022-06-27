using BankPay.API.Models;

namespace BankPay.API.Repositories.OcurrenceRecordRepository
{
    public interface IOcorrenceRecordRepository
    {
        Task<ICollection<OcurrenceRecord>> GetOcurrencesRecord(int id);
        Task<ICollection<OcurrenceRecord>> Statement(int id);
        Task<ICollection<OcurrenceRecord>> OcurrencesRecordYear(int year);
        Task<Account>? FindAccountById(int id);
        Task<Account>? AccountValid(int id, int numberAccount);
    }
}