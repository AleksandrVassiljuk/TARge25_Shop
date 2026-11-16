
using System.ComponentModel.DataAnnotations;

namespace TARge25_Shop.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nimi on kohustuslik")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Linn on kohustuslik")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email on kohustuslik")]
        [EmailAddress(ErrorMessage = "Sisesta korrektne email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parool on kohustuslik")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Parool peab olema vähemalt 6 tähemärki")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kinnita parool")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Paroolid ei ühti")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
