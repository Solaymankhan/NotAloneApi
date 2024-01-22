using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class PostCommentsController : ControllerBase
    {
        int pageSize = 20;

        private ApplicationDbContext _context;

        public PostCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("postPostComments")]
        public async Task<ActionResult> postPostComments([FromForm] PostCommentsModel postComment)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.postComments.AddAsync(postComment);
                await _context.SaveChangesAsync();

                 _context.postsCountingInformation
                  .Where(pci => pci.post_id == postComment.post_id)
                  .ExecuteUpdateAsync(pci => pci.SetProperty(pci => pci.total_comments, pci => pci.total_comments + 1));

                 _context.commentsCountingInformation.AddAsync(new CommentsCountingInformationModel
                                  {comments_id = postComment.comment_id,});

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Successfully Commented");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getPostComments/{postId}/{pageNumber}")]
        public async Task<ActionResult> getPostComments(int postId, int pageNumber = 1)
        {

            try
            {
                var query= _context.postComments
                .Include(postCommentTable => postCommentTable.user)
                .Where(postCommentTable => postCommentTable.post_id == postId)
                .AsQueryable();


                var postsComments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(postsComments);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPut("editPostComment")]
        public async Task<ActionResult> editPostComment([FromForm] PostCommentsModel updatedComment)
        {

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.postComments.Where(pc=>pc.comment_id== updatedComment.comment_id && pc.user_id==updatedComment.user_id)
                    .ExecuteUpdateAsync(pc=>pc.SetProperty(pc=>pc.comment_txt,updatedComment.comment_txt));
                await transaction.CommitAsync();
                return Ok("Comment is Sucessfully Updated");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("deletePostComment/{postId}/{commentId}")]
        public async Task<ActionResult> deletePostComment(int postId, int commentId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                
                Task task1=_context.postComments.Where(pci => pci.comment_id == commentId).ExecuteDeleteAsync();

                Task task2 = _context.postsCountingInformation
                  .Where(pci => pci.post_id == postId)
                  .ExecuteUpdateAsync(pci => pci.SetProperty(pci => pci.total_comments, pci => pci.total_comments - 1));
                Task task3=_context.commentsCountingInformation.Where(pci => pci.comments_id == commentId).ExecuteDeleteAsync();
                Task task4 = _context.commentLikes.Where(pci => pci.comment_id == commentId).ExecuteDeleteAsync();
                Task task5 = _context.commentReplies.Where(pci => pci.comment_id == commentId).ExecuteDeleteAsync();

                await Task.WhenAll(task1, task2, task3, task4, task5);
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
    }
}
