
using Microsoft.AspNetCore.Mvc;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25_Shop.Models.Email;

namespace TARge25_Shop.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailServices _emailServices;

        public EmailController(IEmailServices emailServices)
        {
            _emailServices = emailServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendEmail(EmailViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            var files = Request.Form.Files.Any()
                ? Request.Form.Files.ToList()
                : new List<IFormFile>();

            var emailDto = new EmailDto
            {
                To = vm.To,
                Subject = vm.Subject,
                Body = vm.Body,
                Attachments = files
            };

            _emailServices.SendEmail(emailDto);

            TempData["Success"] = "Email saadetud edukalt!";

            return RedirectToAction(nameof(Index));
        }
    }
}
