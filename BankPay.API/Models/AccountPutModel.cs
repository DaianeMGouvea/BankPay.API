using System.ComponentModel.DataAnnotations;

namespace BankPay.API.Models
{
    public class AccountPutModel
    {
        [Required(ErrorMessage = "Number Account is required")]
        public int Amount { get; set; }
    }
}
