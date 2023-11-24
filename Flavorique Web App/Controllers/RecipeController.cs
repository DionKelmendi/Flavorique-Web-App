using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flavorique_Web_App.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RecipeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Recipe> obj = _db.Recipes.ToList();
            return View(obj);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Recipe obj)
        {
            return View();
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Recipes.Find(id);

            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Recipe obj)
        {
            if (ModelState.IsValid)
            {
                _db.Recipes.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // POST
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Recipes.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.Recipes.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

    }
}
