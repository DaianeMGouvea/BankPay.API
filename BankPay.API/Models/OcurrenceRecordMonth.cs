using BankPay.API.Models.Enums;

namespace BankPay.API.Models
{
    public class OcurrenceRecordMonth
    {
        public int Month { get; set; }
        public double Credits { get; set; }
        public double Debts { get; set; }
        public double Balance { get; set; }

        public OcurrenceRecordMonth(int month)
        {
            Month = month;
        }

        public void BalanceMonth(double amount, TypeRecord typeRecord)
        {
            if (TypeRecord.Credit == typeRecord)
            {
                Credits += amount;
            }
            if (TypeRecord.Debit == typeRecord)
            {
                Debts += amount;
            }

            MonthBalance();

        }

        private void MonthBalance()
        {
            Balance = Credits - Debts;
        }
    }
}
