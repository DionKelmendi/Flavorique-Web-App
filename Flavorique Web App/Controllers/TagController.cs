using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
namespace Flavorique_Web_App.Controllers
{

    public class TagController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TagController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Tag> objTagList = _db.Tags.ToList();
            return View(objTagList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Tag obj)
        {
            if (obj.Name.Trim() == obj.Id.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot be the same as Id. ");
            }

            if (ModelState.IsValid)
            {
                _db.Tags.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    
    // GET
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var tagFromDb = _db.Tags.Find(id);

        if (tagFromDb == null)
        {
            return NotFound();
        }

        return View(tagFromDb);
    }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tag obj)
        {
            if (obj.Name.Trim() == obj.Id.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot be the same as Id.");
            }

            if (ModelState.IsValid)
            {
                _db.Tags.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Tags.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.Tags.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // If you want to implement a POST method for Delete, you can uncomment and modify the following:

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePOST(int? id)
        //{
        //    var obj = _db.Tags.Find(id);

        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }

        //    _db.Tags.Remove(obj);
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}
