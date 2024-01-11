using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flavorique_Web_App.Controllers;

[Route("api/[controller]/[action]")]
public class CommentController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CommentController(ApplicationDbContext db)
    {
        _db = db;
    }


    [HttpGet]
    public async Task<IActionResult> GetComments()
    {
        if (_db.Comments == null)
        {
            return NotFound("There are no comments");
        }

        var comments = await _db.Comments.ToListAsync();

        return Ok(comments);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetComment(int id)
    {
        if (_db.Comments == null)
        {
            return NotFound("There are no comments");
        }
        var comment = await _db.Set<Comment>().Where(x => x.Id == id).FirstOrDefaultAsync();
        if (comment == null)
        {
            return NotFound("Comment not found");
        }
        return Ok(comment);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment(Comment comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment cannot be null");
        }
        
        var commentExists = await _db.Set<Comment>().Where(x => x.Id == comment.Id).FirstOrDefaultAsync();

        if (commentExists == null)
        {
            return BadRequest("Comment doesn't exist");
        }

        commentExists = comment;

        var updatedComment = _db.Set<Comment>().Update(comment);

        if (updatedComment == null)
        {
            return BadRequest("Comment cannot be updated");
        }

        await _db.SaveChangesAsync();

        return Ok(updatedComment);
    }

    [HttpPost]
    public async Task<IActionResult> InsertComment(Comment comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment cannot be null");
        }
        var insertedComment = await _db.Set<Comment>().AddAsync(comment);
        if (insertedComment == null)
        {
            return BadRequest("Comment cannot be inserted");
        }
        await _db.SaveChangesAsync();
        return Ok(insertedComment);
    }


    [HttpDelete("id")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        if (id == 0)
        {
            return BadRequest("Id cannot be 0");
        }
        var comment = await _db.Set<Comment>().Where(x => x.Id == id).FirstOrDefaultAsync();
        if (comment == null)
        {
            return NotFound("Comment not found");
        }
        var deletedComment = _db.Set<Comment>().Remove(comment);
        if (deletedComment == null)
        {
            return BadRequest("Comment cannot be deleted");
        }
        await _db.SaveChangesAsync();
        return Ok(deletedComment);
    }


    [HttpDelete("ids")]
    public async Task<IActionResult> DeleteCommentRange(List<int> ids)
    {
        if (ids == null)
        {
            return BadRequest("Ids cannot be null");
        }
        var comments = await _db.Set<Comment>().Where(x => ids.Contains(x.Id)).ToListAsync();
        if (comments == null)
        {
            return NotFound("Comments not found");
        }
        try
        {
            _db.Set<Comment>().RemoveRange(comments);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
      
        await _db.SaveChangesAsync();
        return Ok("Comments have been deleted");
    }

}
