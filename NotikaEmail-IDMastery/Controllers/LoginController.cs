using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models.IdentityModels;

namespace NotikaEmail_IDMastery.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailContext _context;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            var value = _context.Users.Where(x => x.UserName == model.Username).FirstOrDefault();

            if (value == null)
            {
                ModelState.AddModelError(string.Empty, "Girdiğiniz kullanıcı adı sistemde bulunamadı.");
                return View(model);
            }

            if (!value.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "E-pota adresinizi henüz onaylamamışsınız.");
                return View(model);
            }

            if (value.EmailConfirmed)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true);

                if (result.Succeeded)
                {
                    return RedirectToAction("EditProfile", "Profile");
                }

                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View();
            }

            ModelState.AddModelError(string.Empty, "Lütfen giriş yapmadan önce e-posta adresinizi doğrulayın.");
            return View();
        }
    }
}
