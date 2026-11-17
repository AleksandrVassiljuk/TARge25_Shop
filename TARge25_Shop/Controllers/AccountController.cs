
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Net;
using TARge25_Shop.Core.Domain;
using TARge25_Shop.Models.Account;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25_Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailServices _emailServices;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailServices emailServices,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailServices = emailServices;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                City = model.City
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            await SendConfirmationEmail(user);

            return View("ConfirmationEmailMessage");
        }

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, token = encodedToken },
                Request.Scheme);

            var email = new EmailTokenDto
            {
                To = user.Email!,
                Subject = "SHOP - Confirm your email",
                Body = $@"
                    <h2>Confirm your email</h2>
                    <p>Thank you for registering!</p>
                    <p>
                        <a href=""{WebUtility.HtmlEncode(confirmationLink)}"">
                            Confirm Email
                        </a>
                    </p>"
            };

            try
            {
                _emailServices.SendEmailToken(email, token);

                ViewBag.EmailMessage =
                    "Kinnituskiri saadeti sinu e-postile.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Confirmation email failed.");

                ViewBag.EmailMessage =
                    "Kinnituskirja saatmine ebaõnnestus.";
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Vigane kinnitamise link.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("Kasutajat ei leitud.");

            string decodedToken;

            try
            {
                decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(token));
            }
            catch (FormatException)
            {
                return BadRequest("Vigane kinnitamise token.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] =
                    "E-post kinnitatud! Saad nüüd sisse logida.";

                return RedirectToAction("Login");
            }

            return BadRequest("E-posti kinnitamine ebaõnnestus.");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("",
                    "Palun kinnita kõigepealt oma e-post.");
            }
            else
            {
                ModelState.AddModelError("",
                    "Vale e-post või parool.");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return View("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { email = user.Email, token = encodedToken },
                Request.Scheme);

            var email = new EmailTokenDto
            {
                To = user.Email!,
                Subject = "SHOP - Reset your password",
                Body = $@"
                    <h2>Reset your password</h2>
                    <p>Click the link below to create a new password.</p>
                    <p>
                        <a href=""{WebUtility.HtmlEncode(resetLink)}"">
                            Reset Password
                        </a>
                    </p>"
            };

            try
            {
                _emailServices.SendEmailToken(email, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset email failed.");

                ModelState.AddModelError("",
                    "E-kirja saatmine ebaõnnestus. Proovi hiljem uuesti.");

                return View(model);
            }

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Vigane parooli taastamise link.");

            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return View("ResetPasswordConfirmation");

            string decodedToken;

            try
            {
                decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(model.Token));
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Vigane parooli taastamise link.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Pärast edukat parooli taastamist eemaldame konto lukustuse.
            var unlockResult = await _userManager.SetLockoutEndDateAsync(
                user, null);

            if (!unlockResult.Succeeded)
            {
                _logger.LogWarning(
                    "Password reset succeeded, but unlocking user {UserId} failed.",
                    user.Id);
            }

            var resetCountResult = await _userManager.ResetAccessFailedCountAsync(user);

            if (!resetCountResult.Succeeded)
            {
                _logger.LogWarning(
                    "Password reset succeeded, but clearing failed attempts for {UserId} failed.",
                    user.Id);
            }

            return View("ResetPasswordConfirmation");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
