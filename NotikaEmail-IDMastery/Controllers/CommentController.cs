using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaEmail_IDMastery.Context;
using NotikaEmail_IDMastery.Entities;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles = "Admin")]
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


                var translateRequestBody = new
                {
                    inputs = comment.CommentDetail
                };
                var translateJson = JsonSerializer.Serialize(translateRequestBody);
                var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");

                var translateResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                string englishText = comment.CommentDetail;
                if (translateResponse.IsSuccessStatusCode && translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc = JsonDocument.Parse(translateResponseString);
                    englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }

                var toxicRequestBody = new
                {
                    inputs = englishText
                };


                var toxicJson = JsonSerializer.Serialize(toxicRequestBody);
                var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");
                var toxicResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", toxicContent);
                var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();

                if (toxicResponseString.TrimStart().StartsWith("["))
                {
                    var toxicDoc = JsonDocument.Parse(toxicResponseString);
                    bool isToxic = false;

                    foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
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

        public IActionResult DeleteComment(int id)
        {
            var value = _context.Comments.Find(id);
            _context.Comments.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }

        public IActionResult CommentStatusChangeToToxic(int id)
        {
            var value = _context.Comments.Find(id);
            value.CommentStatus = "Toksik Yorum";
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }

        public IActionResult CommentStatusChangeToPassive(int id)
        {
            var value = _context.Comments.Find(id);
            value.CommentStatus = "Yorum Kaldırıldı";
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }

        public IActionResult CommentStatusChangeToActive(int id)
        {
            var value = _context.Comments.Find(id);
            value.CommentStatus = "Yorum Onaylandı";
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }
    }
}