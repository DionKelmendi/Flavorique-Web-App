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
            if (category == null) { return NotFound(); }
            if (category.Name.Trim() == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot be the same as Display Order.");
                return BadRequest(ModelState);
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
            if (category == null) {  return NotFound(); }
            if (category.Name.Trim() == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot be the same as Display Order.");
                return BadRequest(ModelState);
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