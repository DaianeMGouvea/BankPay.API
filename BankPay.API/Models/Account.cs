using BankPay.API.Models.Enums;

namespace BankPay.API.Models
{
    public class Account
    {
        public int Id { get; private set; }
        public int NumberAccount { get; private set; }
        public double Balance { get; set; } = 0.0;
        public DateTime OpeningAt { get; private set; } = DateTime.Now;
        public List<OcurrenceRecord> OcurrenceRecords { get; set; }

        public Account(int numberAccount)
        {
            NumberAccount = numberAccount;
            OcurrenceRecords = new List<OcurrenceRecord>();
        }

        public void AddCredit(double amount)
        {   
            if (amount < 0.0)
            {
                return;
            }
            Balance += amount;
            OcurrenceRecords.Add(new OcurrenceRecord(TypeRecord.Credit, amount));
        }

        public void Withdraw(double amount)
        {
            if ((Balance - amount) <= 0)
            {
                return;
            }
            Balance -= amount;
            OcurrenceRecords.Add(new OcurrenceRecord(TypeRecord.Debit, amount));
        }

        public List<Account> Statement()
        {
            List<Account> statement = new() { { this } };
            return statement;
        }

        public string MonthlyReport(DateTime date)
        {
            int dateMonth = date.Month;
            return $"Relatorio mensal do mês {dateMonth}";
        }
    }

}
