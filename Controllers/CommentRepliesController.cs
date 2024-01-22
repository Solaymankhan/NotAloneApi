using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class CommentRepliesController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;

        public CommentRepliesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("postCommentReplies")]
        public async Task<ActionResult> postCommentReplies([FromForm] CommentRepliesModel commentReply)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.commentReplies.AddAsync(commentReply);
                await _context.SaveChangesAsync();

                await _context.commentsCountingInformation
                .Where(cci => cci.comments_id == commentReply.comment_id)
                .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.total_replies, cci => cci.total_replies + 1));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Successfully Replied");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("editCommentReplies")]
        public async Task<ActionResult> editCommentReplies([FromForm] CommentRepliesModel commentReply)
        {

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.commentReplies.Where(cr => cr.replied_id == commentReply.replied_id && cr.user_id == commentReply.user_id)
                    .ExecuteUpdateAsync(cr => cr.SetProperty(cr => cr.replied_txt,commentReply.replied_txt));

                await transaction.CommitAsync();
                return Ok("Reply is Sucessfully Updated");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("deleteCommentReplies/{commentId}/{commentReplyId}/{userId}")]
        public async Task<ActionResult> deleteCommentLikes(int commentId, int commentReplyId, int userId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Task task1 = _context.commentReplies
                    .Where(commentLikesTable => commentLikesTable.replied_id == commentReplyId
                    && commentLikesTable.user_id == userId).ExecuteDeleteAsync();

                Task task2 = _context.commentsCountingInformation
                 .Where(cci => cci.comments_id == commentId)
                 .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.total_replies, cci => cci.total_replies - 1));

                Task task3 = _context.commentRepliesLikes
                    .Where(commentRepliesLikes => commentRepliesLikes.comment_reply_id == commentReplyId).ExecuteDeleteAsync();

                await Task.WhenAll(task1, task2,task3);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Comment Reply is Successfully Deleted");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getCommentReplies/{commentId}/{pageNumber}")]
        public async Task<ActionResult> getCommentReplies(int commentId, int pageNumber = 1)
        {

            try
            {
                var query = _context.commentReplies
                    .Include(commentRepliesTable => commentRepliesTable.user)
                    .Where(commentRepliesTable => commentRepliesTable.comment_id == commentId).AsQueryable();

                var commentReplies = await query
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();

                return Ok(commentReplies);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
