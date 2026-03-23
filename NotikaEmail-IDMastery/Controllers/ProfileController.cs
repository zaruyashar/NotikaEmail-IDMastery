using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
