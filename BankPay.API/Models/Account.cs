namespace BankPay.API.Models
{
    public class Account
    {
        public int Id { get; private set; }
        public int NumberAccount { get; private set; }
        public double Balance { get; set; } = 0.0;
        public DateTime OpeningDate { get; private set; } = DateTime.Now;

        

        public Account(int numberAccount)
        {
            NumberAccount = numberAccount;
        }

        public void AddCredit(double credit)
        {   
            if (credit < 0.0)
            {
                return;
            }
            Balance += credit;
        }

        public void Withdraw(double debit)
        {
            if ((Balance - debit) <= 0)
            {
                return;
            }
            Balance -= debit;
        }

        public string Statement()
        {
            return "extrato";
        }

        public string MonthlyReport(DateTime date)
        {
            int dateMonth = date.Month;
            return $"Relatorio mensal do mês {dateMonth}";
        }
    }

}
