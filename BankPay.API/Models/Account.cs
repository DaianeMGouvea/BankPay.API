using BankPay.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPay.API.Models
{
    public class Account
    {
        public int Id { get; private set; }

        [ForeignKey("UserId")]
        public int UserId { get; private set; }
        public int NumberAccount { get; private set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public double Balance { get; set; } = 0.0;

        public DateTime OpeningAt { get; private set; } = DateTime.Now;
        public ICollection<OcurrenceRecord> OccurrenceRecords { get; set; }

        public Account(int numberAccount)
        {
            NumberAccount = numberAccount;
            OccurrenceRecords = new List<OcurrenceRecord>();
        }
       
    }

}
