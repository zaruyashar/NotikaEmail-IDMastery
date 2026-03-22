using Microsoft.AspNetCore.Mvc;

namespace NotikaEmail_IDMastery.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
