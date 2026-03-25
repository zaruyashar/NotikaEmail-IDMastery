using Microsoft.AspNetCore.Mvc;
using NotikaEmail_IDMastery.Context;

namespace NotikaEmail_IDMastery.Controllers
{
    public class ActivationController : Controller
    {
        private readonly EmailContext _context;

        public ActivationController(EmailContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult UserActivation()
        {
            var email = TempData["MoveEmail"] as string;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("UserLogin", "Login");
            }

            TempData["Test1"] = email;
            return View();
        }

        [HttpPost]
        public IActionResult UserActivation(int userCodeParameter)
        {
            var email = TempData["Test1"] as string;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("UserLogin", "Login");
            }

            TempData.Keep("Test1");

            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }

            if (userCodeParameter == user.ActivationCode)
            {
                user.EmailConfirmed = true;
                _context.SaveChanges();
                return RedirectToAction("UserLogin", "Login");
            }

            ModelState.AddModelError(string.Empty, "Girdiğiniz aktivasyon kodu hatalı. Lütfen kontrol edip tekrar deneyin.");
            
            return View();
        }
    }
}