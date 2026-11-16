
using System.ComponentModel.DataAnnotations;

namespace TARge25_Shop.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email on kohustuslik")]
        [EmailAddress(ErrorMessage = "Sisesta korrektne email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parool on kohustuslik")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Jäta mind meelde")]
        public bool RememberMe { get; set; }
    }
}
