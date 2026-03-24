using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models;
using MailKit.Net.Smtp;

namespace NotikaEmail_IDMastery.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterUserViewModel model)
        {
            /* generate activation code */
            Random rnd = new Random();
            int code = rnd.Next(100000, 1000000);

            AppUser appUser = new AppUser()
            {
                Name = model.Name,
                Email = model.Email,
                Surname = model.Surname,
                UserName = model.Username,
                ActivationCode = code
            };
            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (result.Succeeded)
            {
                // email codes here (app psw)
                /* using 'google app passwords' is not the best method here for security reasons;
                 * i'm only following up on course material for now */
                MimeMessage mimeMessage = new MimeMessage();
                MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin","burner email address");
                
                mimeMessage.From.Add(mailboxAddressFrom);

                MailboxAddress mailboxAddressTo = new MailboxAddress("User", model.Email);
                mimeMessage.To.Add(mailboxAddressTo);

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Hesabınızı doğrulamak için gereken aktivasyon kodu: " + code;
                mimeMessage.Body = bodyBuilder.ToMessageBody();
                mimeMessage.Subject = "Notika Identity Aktivasyon Kodu";

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("burner email address", "app psw here");
                client.Send(mimeMessage);
                client.Disconnect(true);


                return RedirectToAction("UserActivation", "Activation");
            }
            else
            {
                /* printing item.Description causes a "user enumeration" risk,
                using item.Code in additional if-else blocks is a potential solution
                for a production environment */
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View();
        }
    }
}
