using Flavorique_Web_App.Data;
using Flavorique_Web_App.DTOs;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentsByRecipe(int id)
    {
        if (_db.Set<Comment>().ToList().Count == 0)
        {
            return NotFound("There are no comments");
        }
        var comments = await _db.Set<Comment>().Where(x => x.RecipeId == id).ToListAsync();
        if (comments == null)
        {
            return NotFound("Comment not found");
        }
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentsByUser(string id, int amount = 3)
    {
        if (_db.Set<Comment>().ToList().Count == 0)
        {
            return NotFound("There are no comments");
        }
        var comments = await _db.Set<Comment>().Where(x => x.AuthorId == id).OrderByDescending(j => j.CreatedDateTime).Take(amount).ToListAsync();
        if (comments == null)
        {
            return NotFound("Comment not found");
        }
        return Ok(comments);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment(UpdateCommentDto comment)
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


        commentExists.Body = comment.Body;
        commentExists.UpdatedDateTime = DateTime.Now;

        var updateComment = _db.Set<Comment>().Update(commentExists);

        _logger.LogInformation("Comment is being updated");

        if (updateComment == null)
        {
            return BadRequest("Comment cannot be updated");
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Comment has been updated");

        var commentResponse = await _db.Set<Comment>().Where(x => x.Id == comment.Id).Include(x => x.Recipe).FirstOrDefaultAsync();

        return Ok(commentResponse);
    }

    [HttpPost]
    public async Task<IActionResult> InsertComment(CreateCommentDto commentToInsert)
    {
        if (commentToInsert == null)
        {
            return BadRequest("Comment cannot be null");
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized("User not found");
        }

        var comment = new Comment
        {
            Body = commentToInsert.Body,
            RecipeId = commentToInsert.RecipeId,
            AuthorId = user?.Id,
            CreatedDateTime = DateTime.UtcNow,
        };

        var insertedComment = await _db.Set<Comment>().AddAsync(comment);
        
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
