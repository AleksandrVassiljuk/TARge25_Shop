
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TARge25_Shop.Models.Email
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "Emaili aadress on kohustuslik")]
        [EmailAddress(ErrorMessage = "Sisesta korrektne email")]
        public string To { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pealkiri on kohustuslik")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sisu on kohustuslik")]
        public string Body { get; set; } = string.Empty;

        public List<IFormFile> Attachments { get; set; } = new();
    }
}
