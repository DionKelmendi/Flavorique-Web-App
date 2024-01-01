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

        public TagController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/Tag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            if (_db.Tags == null)
            {
                return NotFound();
            }
            return await _db.Tags.ToListAsync();
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

// Previous MVC Controller

/* 
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
 
 */