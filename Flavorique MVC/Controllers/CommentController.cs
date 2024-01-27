using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Flavorique_MVC.Models;
using Microsoft.Data.SqlClient;

namespace Flavorique_MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber, string userFilter)
        {
            IEnumerable<Comment> comments = new List<Comment>();
            int pageIndex = 1;
            int totalPages = 1;
            int count = 0;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Comment/GetComments?sortOrder={sortOrder}&searchString={searchString}&pageNumber={pageNumber}&userFilter={userFilter}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Comment>(), pageIndex = 1, totalPages = 1, count = 0 });

                    comments = responseObject.data;
                    pageIndex = responseObject.pageIndex;
                    totalPages = responseObject.totalPages;
                    count = responseObject.count;

                    _logger.LogCritical(responseObject.pageIndex.ToString());
                    _logger.LogCritical(pageIndex.ToString());
                }
            }

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["BodySortParm"] = sortOrder == "body" ? "bodyDesc" : "body";
            ViewData["RateSortParm"] = sortOrder == "rate" ? "rateDesc" : "rate";
            ViewData["DateSortParm"] = sortOrder == "date" ? "dateDesc" : "date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["UserFilterParm"] = userFilter;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            var paginatedList = new PaginatedList<Comment>(comments.ToList(), count, pageIndex, 5);

            return View(paginatedList);
        }

        //GET
        public async Task<IActionResult> Delete(int? id, int? recipeId)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                using (var handler = new HttpClientHandler())
                {
                    var cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Uri("https://localhost:7147"), new Cookie(".AspNetCore.Identity.Application", Request.Cookies[".AspNetCore.Identity.Application"]));

                    handler.CookieContainer = cookieContainer;

                    using (var client = new HttpClient(handler))
                    {
                        {
                            var response = await client.DeleteAsync($"https://localhost:7147/api/Comment/DeleteComment/{id}");
                            string apiResponse = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
               return RedirectToAction("Index", "Recipe");
            }
        }
    }
}
