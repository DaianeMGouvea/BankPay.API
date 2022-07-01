using BankPay.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankPay.API.Models
{
    public class OcurrenceRecordMonth
    {
        public int Month { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public double Credits { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public double Debts { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public double Balance { get; set; }

        public OcurrenceRecordMonth(int month)
        {
            Month = month;
        }

        public void MonthBalance(double amount, TypeRecord typeRecord)
        {
            if (TypeRecord.Credit == typeRecord)
                Credits += amount;

            else if (TypeRecord.Debit == typeRecord)
                Debts += amount;
            else
                return;

            Balance = Credits - Debts;
        }
    }
}
