using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;

namespace Flavorique_Web_App.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RecipeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            IEnumerable<Recipe> obj = _db.Recipes.ToList();
            return View(obj);
        }

        //GET
        public IActionResult Details(int? id)
        {
            var obj = _db.Recipes.Find(id);

            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //GET
        public IActionResult Create()
        {
            if (_signInManager.IsSignedIn(User)) {
                return View();
            }
            return RedirectToAction("Index");
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe obj)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var recipeFromDb = _db.Recipes.FirstOrDefault(c => c.Title == obj.Title); 

                if (recipeFromDb != null) {
                    ModelState.AddModelError("Title", "Title already exists.");
                    return View(obj);
                }

                var content = obj.Body.Replace("&nbsp; ", "");

                if (content.Length < 100)
                {
                    ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                    return View(obj);
                }

                obj.AuthorId = user.Id;

                string fillerString = "image widget. Press Enter to type after or press Shift + Enter to type before the widget";
                obj.Body = obj.Body.Replace(fillerString, ""); ;

                _db.Recipes.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var obj = _db.Recipes.Find(id);

            if (obj == null || userId != obj.AuthorId)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recipe obj)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var recipeFromDb = _db.Recipes.FirstOrDefault(c => c.Title == obj.Title);

                if (recipeFromDb != null)
                {
                    ModelState.AddModelError("Title", "Title already exists.");
                    return View(obj);
                }

                var content = obj.Body.Replace("&nbsp; ", "");

                if (content.Length < 100)
                {
                    ModelState.AddModelError("Body", "Recipe is too short or is empty.");
                    return View(obj);
                }

                obj.AuthorId = user.Id;

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
