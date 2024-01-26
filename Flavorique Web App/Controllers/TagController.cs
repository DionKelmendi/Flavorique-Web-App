using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore;

namespace Flavorique_Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TagController> _logger;

        public TagController(ApplicationDbContext db, ILogger<TagController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: api/Tag
        [HttpGet]
        public async Task<ActionResult<PaginatedList<Tag>>> GetTags(string? sortOrder, string? searchString, int? pageNumber, int pageSize = 5, string? category = "")
        {
            if (_db.Tags == null)
            {
                return NotFound();
            }
            IEnumerable<Tag> tags = await _db.Tags.Include(t => t.Category).ToListAsync();

            if (!String.IsNullOrEmpty(category))
            {
                tags = tags.Where(t => t.Category.Name.ToLower().Equals(category));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                tags = tags.Where(t => t.Name.ToLower().Contains(searchString.ToLower()) || t.Category.Name.ToLower().Contains(searchString.ToLower()));
            }

            int count = tags.Count();

            switch (sortOrder)
            {
                case "name":
                    tags = tags.OrderBy(t => t.Name);
                    break;
                case "nameDesc":
                    tags = tags.OrderByDescending(t => t.Name);
                    break;
                case "category":
                    tags = tags.OrderBy(t => t.Category?.Name);
                    break;
                case "categoryDesc":
                    tags = tags.OrderByDescending(t => t.Category?.Name);
                    break;
                case "idDesc":
                    tags = tags.OrderByDescending(t => t.Id);
                    break;
                default:
                    tags = tags.OrderBy(t => t.Id);
                    break;
            }

            PaginatedList<Tag> result = await PaginatedList<Tag>.CreateAsync(tags, pageNumber ?? 1, pageSize);

            _logger.LogInformation(result.ToString());

            // Return an object that includes both the paginated list and pagination information
            return Ok(new { data = result, pageIndex = result.PageIndex, totalPages = result.TotalPages, count = count });
        }

        // GET: api/Tag/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            if (_db.Tags == null)
            {
                return NotFound();
            }
            var tag = await _db.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }
            return tag;
        }

        // GET: api/Tag
        [HttpGet("name")]
        public ActionResult<IEnumerable<string>> GetTagsName()
        {
            if (_db.Tags == null)
            {
                return NotFound();
            }

            var result = _db.Tags.Select(i => i.Name).OrderBy(j => j).ToList(); // Convert the result to a list of strings

            return result;
        }

        // PUT: api/Tag/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            var nameExists = await _db.Tags.AnyAsync(e => e.Name == tag.Name && e.Id != tag.Id);

            if (nameExists)
            {
                ModelState.AddModelError("Name", "Tag already exists.");
                return BadRequest(ModelState);
            }

            _db.Entry(tag).State = EntityState.Modified;

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

        // POST: api/Tag
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            if (_db.Tags == null)
            {
                return Problem("Entity set 'db.Tags' is null.");
            }
            var nameExists = await _db.Tags.AnyAsync(e => e.Name == tag.Name);

            if (nameExists)
            {
                ModelState.AddModelError("Name", "Tag already exists.");
                return BadRequest(ModelState);
            }

            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, tag);
        }

        // DELETE: api/Tag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            if (_db.Tags == null)
            {
                return NotFound();
            }
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool Exists(int id)
        {
            return (_db.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
