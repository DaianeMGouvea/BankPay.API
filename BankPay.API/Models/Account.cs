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

        public ICollection<Account> Statement()
        {
            List<Account> statement = new() { { this } };
            return statement;
        }

        public ICollection<OcurrenceRecordMonth> OcurrenceRecordYear(int year)
        {

            return ReturnRecordMonth(OcurrenceRecords.Where(o => o.CreatedAt.Year == year));
        }

        private ICollection<OcurrenceRecordMonth> ReturnRecordMonth(IEnumerable<OcurrenceRecord> FilteredRecords)
        {
            int currentMonth = 0;
            int index = 0;

            List<OcurrenceRecordMonth> ocurrencesRecordMonth = new();

            foreach (var record in FilteredRecords)
            {
                if (currentMonth == 0)
                {
                    currentMonth = record.CreatedAt.Month;
                    ocurrencesRecordMonth.Add(new(record.CreatedAt.Month));
                }

                if (currentMonth != record.CreatedAt.Month)
                {
                    currentMonth = record.CreatedAt.Month;
                    ocurrencesRecordMonth.Add(new(record.CreatedAt.Month));
                    index++;

                }

                ocurrencesRecordMonth[index].BalanceMonth(record.Amount, record.TypeRecord);
            }
            return ocurrencesRecordMonth;
        }
    }

}
