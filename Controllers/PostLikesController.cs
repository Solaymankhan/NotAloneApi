using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class PostLikesController : ControllerBase
    {
        int pageSize = 20;

        private ApplicationDbContext _context;

        public PostLikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postPostLikes")]
        public async Task<ActionResult> postPostLikes([FromForm] PostLikesModel postLike)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.postLikes.AddAsync(postLike);
                await _context.SaveChangesAsync();
                _context.postsCountingInformation
                 .Where(pci => pci.post_id == postLike.post_id)
                 .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_likes, pci => pci.total_likes + 1));
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Successfully Liked");

            }
            catch(Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("deletePostLikes/{postId}/{userId}")]
        public async Task<ActionResult> deletePostLikes(int postId, int userId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Task task1 = _context.postLikes
                    .Where(postLikesTable => postLikesTable.post_id==postId
                    && postLikesTable.user_id ==userId).ExecuteDeleteAsync();

                Task task2=_context.postsCountingInformation
                 .Where(pci => pci.post_id == postId)
                 .ExecuteUpdateAsync(pci => pci.SetProperty(pci => pci.total_likes, pci => pci.total_likes - 1));

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

        [HttpGet("getPostLikes/{postId}/{pageNumber}")]
        public async Task<ActionResult> getPostLikes(int postId, int pageNumber = 1)
        {

            try
            {
                var query = _context.postLikes
                    .Include(postLikesTable=> postLikesTable.user)
                    .Where(postLikesTable => postLikesTable.post_id== postId).AsQueryable();

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
