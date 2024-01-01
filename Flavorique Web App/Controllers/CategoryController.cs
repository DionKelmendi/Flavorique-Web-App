using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flavorique_Web_App.Models;
using Flavorique_Web_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Flavorique_Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_db.Categories == null)
            {
                return NotFound();
            }
            return await _db.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (_db.Categories == null)
            {
                return NotFound();
            }
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        // GET: api/Categories
        [HttpGet("name")]
        public ActionResult<IEnumerable<string>> GetCategoriesName()
        {
            if (_db.Categories == null)
            {
                return NotFound();
            }

            var result = _db.Categories.Select(i => i.Name).OrderBy(j => j).ToList(); // Convert the result to a list of strings

            return result;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _db.Entry(category).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (_db.Categories == null)
            {
                return Problem("Entity set 'db.Categories' is null.");
            }
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_db.Categories == null)
            {
                return NotFound();
            }
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool Exists(int id)
        {
            return (_db.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}

// Previous MVC Controller
/*
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
*/