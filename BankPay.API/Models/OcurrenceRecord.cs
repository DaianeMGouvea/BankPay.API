using BankPay.API.Models.Enums;

namespace BankPay.API.Models
{
    public class OcurrenceRecord
    {
        public int Id { get; private set; }
        public TypeRecord TypeRecord { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        public OcurrenceRecord(TypeRecord typeRecord, double amount)
        {
            TypeRecord = typeRecord;
            Amount = amount;
        }
    }
}
