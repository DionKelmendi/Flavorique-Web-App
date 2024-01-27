using System.Text.RegularExpressions;
using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Flavorique_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return Redirect("/swagger/");
        }

        [HttpGet("api/Home/")]
        public async Task<IActionResult> GetHomeData()
        {
            var userCount = _userManager.Users.Count();
            var categoryCount = _db.Categories.Count();
            var tagCount = _db.Tags.Count();
            var recipeCount = _db.Recipes.Count();
            float averageRating = 0;

            var ratings = _db.Comments.Select(c => c.Rating).ToList();

            var ratingGraphData = new HomeRatingGraphData
            {
                One = ratings.Count(r => r == 1),
                Two = ratings.Count(r => r == 2),
                Three = ratings.Count(r => r == 3),
                Four = ratings.Count(r => r == 4),
                Five = ratings.Count(r => r == 5),
            };

            foreach (var rating in ratings) {
                averageRating += rating;
            }
            averageRating /= ratings.Count();


            IEnumerable<ShortRecipe> mostRecentRecipes = _db.Recipes.Include(r => r.Author)
                .Select(i => new ShortRecipe
                {
                    Id = i.Id,
                    Title = i.Title,
                    AuthorId = i.AuthorId,
                    AuthorName = i.Author.UserName,
                    CreatedDateTime = i.CreatedDateTime,
                    Body = StripHtmlTags(i.Body).Length > 200 ? StripHtmlTags(i.Body).Substring(0, 200) : StripHtmlTags(i.Body),
                    Image = GetImageFromHtml(i.Body)
                })
                .OrderByDescending(j => j.CreatedDateTime)
                .Take(3)
                .ToList();

            var tagData = _db.RecipeTags
                .Include(r => r.Tag)
                .AsEnumerable()
                .GroupBy(r => r.Tag)
                .OrderByDescending(group => group.Count())
                .Take(10)
                .Select(group => new TagGraphItem
                {
                    Tag = group.Key,
                    Count = group.Count()
                })
                .ToList();

            var model = new HomeViewModel
            {
                UserCount = userCount,
                CategoryCount  = categoryCount,
                TagCount  = tagCount,
                RecipeCount = recipeCount,
                AverageRating  = averageRating.ToString("0.00"),
                GraphData  = ratingGraphData,
                MostRecentRecipes  = mostRecentRecipes,
                TagGraphItems  = tagData,
                MostUsedTag  = tagData[0]
            };

            return Ok(model);
        }

        [HttpGet("logo")]
        public IActionResult GetLogo()
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo.png");

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound(); 
            }

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/png");
        }

        public async Task<IActionResult> ProvaMailer()
        {
            var reciever = "dionkelmendi@hotmail.com";
            var subject = "Prova";
            var message = "Hello!!!!";

            await _emailSender.SendEmailAsync(reciever, subject, message);

            return RedirectToAction("Index");
        }
        private static string GetImageFromHtml(string htmlContent)
        {
            string pattern = @"src=""([^""]*)""";
            Match match = Regex.Match(htmlContent, pattern);

            if (match.Success)
            {
                var image = match.Value.Substring(5);
                image = image.Remove(image.Length - 1, 1);

                return match.Groups[1].Value;
            }

            // Please make an URL for the Website Logo
            return "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/_9A8c_VZOve3/images/377.png";
            /*
             * This does return the Logo, but since the site is local, it cannot be seen by e-mails
            return "https://localhost:7147/logo";
            */
        }

        private static string StripHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}