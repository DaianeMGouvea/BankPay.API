using System.ComponentModel.DataAnnotations;

namespace BankPay.API.Models
{
    public class UserPostModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
