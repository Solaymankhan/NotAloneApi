using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class PostSharesController : ControllerBase
    {


        private ApplicationDbContext _context;

        public PostSharesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("postPostShares")]
        public async Task<ActionResult> postPostShares([FromForm] PostModel post)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.posts.AddAsync(post);
                await _context.SaveChangesAsync();
                _context.postsCountingInformation
                 .Where(pci => pci.post_id ==post.shared_post_id)
                 .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_shares, pci => pci.total_shares + 1));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Post is Successfully Shared");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("editSharedPost")]
        public async Task<ActionResult> editSharedPost([FromForm] PostModel postInfo)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var post=await _context.posts.Where(sp => sp.post_id == postInfo.post_id && sp.user_id == postInfo.user_id).FirstOrDefaultAsync();

                if (post != null)
                {
                        post.text = postInfo.text;
                        post.place = postInfo.place;
                }


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Post is Sucessfully Updated");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("deletSharedPost/{oldPostId}/{newPostId}/{userId}")]
        public async Task<ActionResult> deleteReplyLikes(int oldPostId, int newPostId, int userId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Task task1 = _context.posts
                    .Where(shares => shares.post_id == newPostId
                    && shares.user_id == userId).ExecuteDeleteAsync();

                Task task2 = _context.postsCountingInformation
                 .Where(ci => ci.post_id == oldPostId)
                 .ExecuteUpdateAsync(ci => ci.SetProperty(ci => ci.total_shares, cr => cr.total_shares - 1));

                await Task.WhenAll(task1, task2);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Shared post is Deleted Successfully");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }

    }
}
