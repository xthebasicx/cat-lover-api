using cat_lover_api.Data;
using cat_lover_api.Models.DTOs;
using cat_lover_api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cat_lover_api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<List<Comment>> GetComment()
        {
            var comments = await _context.Comments.ToListAsync();
            return comments;
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userId == null) return Unauthorized("User not authorized");

            Comment comment = new()
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userEmail!
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment created successfully", comment.Id });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] CommentDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized("User not authorized");

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null) return NotFound(new { Message = "Comment not found" });
            if (comment.AuthorId != userId) return Forbid("You are not allowed to edit this comment");

            comment.Content = request.Content;
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized("User not authorized");

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null) return NotFound(new { Message = "Comment not found" });

            if (comment.AuthorId != userId) return Forbid("You are not allowed to delete this comment");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment deleted successfully" });
        }
    }
}
