using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaEmail_IDMastery.Context;

namespace NotikaEmail_IDMastery.Controllers
{
    public class CommentController : Controller
    {
        private readonly EmailContext _context;

        public CommentController(EmailContext context)
        {
            _context = context;
        }

        public IActionResult UserComments()
        {
            var values = _context.Comments.Include(x => x.AppUser).ToList();
            return View(values);
        }

        public IActionResult UserCommentList()
        {
            var values = _context.Comments.Include(x => x.AppUser).ToList();
            return View(values);
        }
    }
}
