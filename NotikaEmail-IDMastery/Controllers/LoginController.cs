using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using NotikaEmail_IDMastery.Models;

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
            var value = _context.Users.Where(x=> x.UserName == model.Username)
                .FirstOrDefault();
            if (value.EmailConfirmed == true)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "MyProfile");
                }
                return View();
            }
            return View();
        }
    }
}
