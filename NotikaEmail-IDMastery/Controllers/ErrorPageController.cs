using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.Controllers
{
    public class ErrorPageController : Controller
    {
        public IActionResult Page404()
        {
            return View();
        }
    }
}
