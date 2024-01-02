using Flavorique_MVC.Models;
using Flavorique_MVC.Data;
using Microsoft.AspNetCore.Mvc;

namespace Flavorique_MVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name.Trim() == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot be the same as Display Order.");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Returns first object from sequence
            // var categoryFromDbFirst = _db.Categories.FirstOrDefault(c => c.Id == id); 

            // Returns object from sequence where the condition is satisfied, returns exception if more than one object is found
            // var categoryFromDbSingle = _db.Categories.SingleOrDefault(c => c.Id == id); 

            var categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name.Trim() == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot be the same as Display Order.");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
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

            var obj = _db.Categories.Find(id);

            if (obj == null)
            {
                return NotFound();
            }
            //  return View(categoryFromDb);

            _db.Categories.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        //POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePOST(int? id)
        //{
        //  var obj = _db.Categories.Find(id);

        //if (obj == null) {
        //  return NotFound();
        //}

        //_db.Categories.Remove(obj);
        //_db.SaveChanges();
        //return RedirectToAction("Index");
        // }
    }
}
