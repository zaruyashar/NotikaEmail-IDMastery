using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using System.Text.Json;
using System.Text;

namespace NotikaEmail_IDMastery.Controllers
{
    public class CommentController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public CommentController(EmailContext context, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
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

        [HttpGet]
        public PartialViewResult CreateComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }

            comment.AppUserId = user.Id;
            comment.CommentDate = DateTime.Now;


            // WARNING: Instantiating a new HttpClient per request can cause socket exhaustion (TIME_WAIT) under heavy load.
            // For production, refactor this to use IHttpClientFactory via Dependency Injection.
            using (var client = new HttpClient())
            {
                var apiKey = _configuration["HuggingFace:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    ModelState.AddModelError(string.Empty, "API anahtarı bulunamadı. Yapılandırmayı kontrol edin.");
                    return PartialView(comment);
                }

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var requestBody = new { inputs = comment.CommentDetail };
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var doc = JsonDocument.Parse(responseString);
                    bool isToxic = false;

                    foreach (var item in doc.RootElement[0].EnumerateArray())
                    {
                        string label = item.GetProperty("label").GetString();
                        double score = item.GetProperty("score").GetDouble();

                        if (label.Equals("toxic", StringComparison.OrdinalIgnoreCase) && score > 0.5)
                        {
                            isToxic = true;
                            break;
                        }
                    }

                    if (isToxic)
                    {
                        comment.CommentStatus = "Toksik Yorum";
                    }
                    else
                    {
                        comment.CommentStatus = "Onay Bekliyor";
                    }
                }
                else
                {
                    comment.CommentStatus = "Onay Bekliyor - API Sorunu";
                }
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }
    }
}