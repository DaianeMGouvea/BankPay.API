using System.ComponentModel.DataAnnotations;

namespace BankPay.API.Models
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Phone { get; set; }
        public Account Account { get; private set; }

        private int _random = new Random().Next(10000, 99999);

        public User()
        {

        }

        public User(UserPostModel user)
        {
            Name = user.Name;
            Phone = user.Phone;
            Cpf = user.Cpf;
            Account = new Account(_random);
        }

        public void applyChanges(UserPutModel u)
        {
            Name = u.Name;
            Phone = u.Phone;
        }
    }
}
