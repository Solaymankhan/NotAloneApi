using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class CommentRepliesLikesController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;

        public CommentRepliesLikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postRelyLikes")]
        public async Task<ActionResult> postRelyLikes([FromForm] CommentRepliesLikesModel commentReplyLike)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.commentRepliesLikes.AddAsync(commentReplyLike);
                await _context.SaveChangesAsync();
                _context.commentReplies
                 .Where(cr => cr.replied_id == commentReplyLike.comment_reply_id)
                 .ExecuteUpdateAsync(cr => cr.SetProperty(cr => cr.total_likes, cr => cr.total_likes + 1));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Successfully Liked");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("deleteReplytLikes/{commentId}/{userId}")]
        public async Task<ActionResult> deleteReplyLikes(int replyId, int userId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Task task1 = _context.commentRepliesLikes
                    .Where(commentReplyTable => commentReplyTable.comment_reply_id == replyId
                    && commentReplyTable.user_id == userId).ExecuteDeleteAsync();

                Task task2 = _context.commentReplies
                 .Where(cr => cr.replied_id == replyId)
                 .ExecuteUpdateAsync(cr => cr.SetProperty(cr => cr.total_likes, cr => cr.total_likes - 1));

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


        [HttpGet("getReplyLikes/{replyId}/{pageNumber}")]
        public async Task<ActionResult> getRelyLikes(int replyId, int pageNumber = 1)
        {

            try
            {
                var query = _context.commentRepliesLikes
                    .Include(replyLikesTable => replyLikesTable.user)
                    .Where(replyLikesTable => replyLikesTable.comment_reply_id == replyId).AsQueryable();

                var repliedLikes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                return Ok(repliedLikes);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
