using BankPay.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPay.API.Models
{
    public class OcurrenceRecord
    {
        public int Id { get; private set; }

        [ForeignKey("AccountId")]
        public int AccountId { get; private set; }
        public TypeRecord TypeRecord { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public double Amount { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        public OcurrenceRecord(TypeRecord typeRecord, double amount, int accountId)
        {
            TypeRecord = typeRecord;
            Amount = amount;
            AccountId = accountId;
        }
    }
}
