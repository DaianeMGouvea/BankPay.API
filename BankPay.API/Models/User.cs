using System.ComponentModel.DataAnnotations;

namespace BankPay.API.Models
{
    public class User
    {
        public int Id { get; private set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Phone { get; set; }
        public Account Account { get; private set; }

        private int _random = new Random().Next(10000, 99999);

        public User(string name, string phone = "(00)0000-0000")
        {
            Name = name;
            Phone = phone;
            Account = new Account(_random);
        }


    }
}
