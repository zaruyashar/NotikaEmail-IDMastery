using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models.ForgotPasswordModels;
using MailKit.Net.Smtp;

namespace NotikaEmail_IDMastery.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public PasswordChangeController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(forgotPasswordViewModel.Email))
            {
                return View(forgotPasswordViewModel);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);

            if (user == null)
            {
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResetTokenLink = Url.Action("ResetPassword", "PasswordChange", new
            {
                userId = user.Id,
                token = passwordResetToken
            }, HttpContext.Request.Scheme);

            try
            {
                string senderEmail = _configuration["EmailSettings:SenderEmail"];
                string senderPassword = _configuration["EmailSettings:AppPassword"];

                MimeMessage mimeMessage = new MimeMessage();

                MailboxAddress mailboxAddressFrom = new MailboxAddress("Notika Admin", senderEmail);
                mimeMessage.From.Add(mailboxAddressFrom);

                MailboxAddress mailboxAddressTo = new MailboxAddress("User", forgotPasswordViewModel.Email);
                mimeMessage.To.Add(mailboxAddressTo);

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Şifrenizi yenilemek için şu bağlantıya tıklayın: " + passwordResetTokenLink;
                mimeMessage.Body = bodyBuilder.ToMessageBody();
                mimeMessage.Subject = "Şifre Değişiklik Talebi";

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate(senderEmail, senderPassword);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "E-posta gönderilirken sistemde bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(forgotPasswordViewModel);
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                ViewBag.v = "Bir Hata Oluştu";
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.ResetPasswordAsync(user, token.ToString(), resetPasswordViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }

            return View();
        }
    }
}