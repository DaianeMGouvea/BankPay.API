using BankPay.API.Models;

namespace BankPay.API.Repositories
{
    public interface IOcorrenceRecord
    {
        Task<ICollection<OcurrenceRecord>> Statement();
        Task<ICollection<OcurrenceRecord>> OcurrenceRecordYear();
    }
}
