using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NotAlone.Controllers
{

    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class CommentLikesController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;

        public CommentLikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postCommentLikes")]
        public async Task<ActionResult> postCommentLikes([FromForm] CommentsLikesModel commentLike)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.commentLikes.AddAsync(commentLike);
                await _context.SaveChangesAsync();
                _context.commentsCountingInformation
                 .Where(cci => cci.comments_id == commentLike.comment_id)
                 .ExecuteUpdate(cci => cci.SetProperty(cci => cci.total_likes, cci => cci.total_likes + 1));
            
                await transaction.CommitAsync();
                return Ok("Successfully Liked");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("deleteCommentLikes/{commentId}/{userId}")]
        public async Task<ActionResult> deleteCommentLikes(int commentId, int userId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Task task1 = _context.commentLikes
                    .Where(commentLikesTable => commentLikesTable.comment_id == commentId
                    && commentLikesTable.user_id == userId).ExecuteDeleteAsync();

                Task task2 = _context.commentsCountingInformation
                 .Where(cci => cci.comments_id == commentId)
                 .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.total_likes, cci => cci.total_likes - 1));

                await Task.WhenAll(task1, task2);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Successfully DisLiked");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("getCommentLikes/{commentId}/{pageNumber}")]
        public async Task<ActionResult> getCommentLikes(int commentId, int pageNumber = 1)
        {

            try
            {
                var query = _context.commentLikes
                    .Include(commentLikesTable => commentLikesTable.user)
                    .Where(commentLikesTable => commentLikesTable.comment_id == commentId).AsQueryable();


                var postsLikes = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

                return Ok(postsLikes);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
