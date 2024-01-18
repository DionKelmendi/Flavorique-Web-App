using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Flavorique_MVC.Controllers
{
    public class CommentController : Controller
    {
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

                return RedirectToAction("Details", "Recipe" , new { id = recipeId });
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
               return RedirectToAction("Details", "Recipe");
            }
        }
    }
}
