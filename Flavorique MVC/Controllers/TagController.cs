using Microsoft.AspNetCore.Mvc;
using Flavorique_MVC.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.Data.SqlClient;

namespace Flavorique_MVC.Controllers
{
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;

        public TagController(ILogger<TagController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber)
        {
            IEnumerable<Tag> tags = new List<Tag>();
            int pageIndex = 1;
            int totalPages = 1;
            int count = 0;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Tag?sortOrder={sortOrder}&searchString={searchString}&pageNumber={pageNumber}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeAnonymousType(apiResponse, new { data = Enumerable.Empty<Tag>(), pageIndex = 1, totalPages = 1, count = 0 });

                    tags = responseObject.data;
                    pageIndex = responseObject.pageIndex;
                    totalPages = responseObject.totalPages;
                    count = responseObject.count;

                    _logger.LogCritical(responseObject.pageIndex.ToString());
                    _logger.LogCritical(pageIndex.ToString());
                }
            }

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "idDesc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "nameDesc" : "name";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            _logger.LogWarning(tags.Count().ToString());

            var paginatedList = new PaginatedList<Tag>(tags.ToList(), count, pageIndex, 5);

            return View(paginatedList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("https://localhost:7147/api/Tag",
                        new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, tag);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(tag);
            }
        }

        // GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var tag = new Tag();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://localhost:7147/api/Tag/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        tag = JsonConvert.DeserializeObject<Tag>(result);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(tag);
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tag tag)
        {
            try
            {
                var id = tag.Id;

                using (var client = new HttpClient())
                {
                    var response = await client.PutAsync($"https://localhost:7147/api/Tag/{id}",
                        new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8, "application/json"));

                    return await HandleResponse(response, tag);
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(tag);
            }
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response, Tag tag)
        {
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Handle validation errors from the API
                var validationErrors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(result);

                foreach (var error in validationErrors)
                {
                    foreach (var errorMessage in error.Value)
                    {
                        ModelState.AddModelError(error.Key, errorMessage);
                    }
                }

                return View(tag);
            }

            // Handle other non-success status codes
            ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
            return View(tag);
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                using (var client = new HttpClient())
                {
                    using (var response = await client.DeleteAsync($"https://localhost:7147/api/Tag/{id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View();
            }
        }
    }
}
