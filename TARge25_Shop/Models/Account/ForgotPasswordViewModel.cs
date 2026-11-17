
using System.ComponentModel.DataAnnotations;

namespace TARge25_Shop.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
