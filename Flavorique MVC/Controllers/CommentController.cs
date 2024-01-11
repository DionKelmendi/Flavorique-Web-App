using Microsoft.AspNetCore.Mvc;

namespace Flavorique_MVC.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
