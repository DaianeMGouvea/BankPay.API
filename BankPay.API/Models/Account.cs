using BankPay.API.Models.Enums;

namespace BankPay.API.Models
{
    public class Account
    {
        public int Id { get; private set; }
        public int NumberAccount { get; private set; }
        public double Balance { get; set; } = 0.0;
        public DateTime OpeningAt { get; private set; } = DateTime.Now;
        public ICollection<OcurrenceRecord> OcurrenceRecords { get; set; }

        public Account(int numberAccount)
        {
            NumberAccount = numberAccount;
            OcurrenceRecords = new List<OcurrenceRecord>();
        }
       
    }

}
