using BankPay.API.Models;

namespace BankPay.API.Repositories.OcurrenceRecordRepository
{
    public interface IOcorrenceRecordRepository
    {
        Task<ICollection<OcurrenceRecord>> Statement();
        Task<ICollection<OcurrenceRecord>> OcurrenceRecordYear(int year);
    }
}