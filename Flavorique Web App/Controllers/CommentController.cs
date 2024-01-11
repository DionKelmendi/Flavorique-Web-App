using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flavorique_Web_App.Controllers;

[Route("api/[controller]/[action]")]
public class CommentController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CommentController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public CommentController(ApplicationDbContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<CommentController> logger)
    {
        _db = db;
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> GetComments()
    {
        if (_db.Set<Comment>().ToList().Count == 0)
        {
            return NotFound("There are no comments");
        }

        var comments = await _db.Set<Comment>().ToListAsync();

        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComment(int id)
    {
        if (_db.Set<Comment>().ToList().Count == 0)
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

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized("User not found.");
        } 
        
        var commentExists = await _db.Set<Comment>().Where(x => x.Id == comment.Id).FirstOrDefaultAsync();

        if (commentExists == null)
        {
            return BadRequest("Comment doesn't exist");
        }

        if(commentExists?.AuthorId != user?.Id)
        {
            return Unauthorized("This is not your comment");
        }

        commentExists = comment;

        var updatedComment = _db.Set<Comment>().Update(comment);

        _logger.LogInformation("Comment is being updated");

        if (updatedComment == null)
        {
            return BadRequest("Comment cannot be updated");
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment has been updated");

        return Ok(updatedComment);
    }

    [HttpPost]
    public async Task<IActionResult> InsertComment(Comment comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment cannot be null");
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized("User not found");
        }

        var commentToInsert = new Comment
        {
            Body = comment.Body,
            RecipeId = comment.RecipeId,
            AuthorId = user?.Id,
            CreatedDateTime = DateTime.UtcNow,
        };

        var insertedComment = await _db.Set<Comment>().AddAsync(commentToInsert);
        
        _logger.LogInformation("Comment is being insterted");

        if (insertedComment == null)
        {
            return BadRequest("Comment cannot be inserted");
        }
        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment has been successfully inserted");

        return Ok(insertedComment);
    }
    

    [HttpDelete("{id}")]
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

        var user = await _userManager.GetUserAsync(User);

        if(user == null)
        {
            return Unauthorized("User not found");

        }
        if (comment.AuthorId != user.Id)
        {
            return Unauthorized("This comment does not belong to the user");
        }

        var deletedComment = _db.Set<Comment>().Remove(comment);

        _logger.LogInformation("Comment is being removed");

        if (deletedComment == null)
        {
            return BadRequest("Comment cannot be deleted");
        }
        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment has been successfully removed");

        return Ok(deletedComment);
    }



}
