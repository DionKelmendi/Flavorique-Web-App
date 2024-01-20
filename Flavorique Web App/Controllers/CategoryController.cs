using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flavorique_Web_App.Models;
using Flavorique_Web_App.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace Flavorique_Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext db, ILogger<CategoryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<PaginatedList<Category>>> GetCategories(string? sortOrder, string? searchString, int? pageNumber)
        {
            if (_db.Categories == null)
            {
                return NotFound();
            }

            IEnumerable<Category> categories = await _db.Categories.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(searchString.ToLower()));
            }
            int count = categories.Count();

            switch (sortOrder)
            {
                case "name":
                    categories = categories.OrderBy(c => c.Name);
                    break;
                case "nameDesc":
                    categories = categories.OrderByDescending(c => c.Name);
                    break;
                case "date":
                    categories = categories.OrderBy(c => c.CreatedDateTime);
                    break;
                case "dateDesc":
                    categories = categories.OrderByDescending(c => c.CreatedDateTime);
                    break;
                case "idDesc":
                    categories = categories.OrderByDescending(c => c.Id);
                    break;
                default:
                    categories = categories.OrderBy(c => c.Id);
                    break;
            }

            int pageSize = 5;
            PaginatedList<Category> result = await PaginatedList<Category>.CreateAsync(categories, pageNumber ?? 1, pageSize);

            _logger.LogInformation(result.ToString());

            return Ok(new { data = result, pageIndex = result.PageIndex, totalPages = result.TotalPages, count = count });
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

            var result = _db.Categories.Select(i => i.Name).OrderBy(j => j).ToList(); 

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

            _db.Entry(category).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!Exists(id))
                {
                    return NotFound(ex.Message);
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

        [HttpGet("tags")]
        public async Task<IActionResult> GetCategoriesWithTags()
        {
            var categoriesWithTags = _db.Categories
            .Select(category => new CategoryViewModel
            {
                Category = category,
                Tags = _db.Tags
                    .Where(tag => tag.CategoryId == category.Id)
                    .ToList()
            })
            .ToList();

            return Ok(categoriesWithTags);
        }

        private bool Exists(int id)
        {
            return (_db.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}