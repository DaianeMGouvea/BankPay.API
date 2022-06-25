using BankPay.API.Models;

namespace BankPay.API.Repositories.OcurrenceRecordRepository
{
    public interface IOcorrenceRecordRepository
    {
        Task<ICollection<OcurrenceRecord>> GetOcurrencesRecord();
        Task<ICollection<OcurrenceRecord>> Statement();
        Task<ICollection<OcurrenceRecord>> OcurrencesRecordYear(int year);
        Task<Account>? FindByNumberAccount(int numberAccount);
    }
}