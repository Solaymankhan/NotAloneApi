using NotAlone.Data;
using NotAlone.Models;
using NotAlone.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles ="User,Admin")]
    public class PostController : ControllerBase
    {
        int pageSize = 20;

        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public PostController(IFileService fileService,ApplicationDbContext context)
        {
            _fileService = fileService;
            _context=context;
        }



        [DisableRequestSizeLimit]
        [HttpPost("postPost")]
        public async Task<ActionResult> postPost([FromForm] PostModel post)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                string? filestr=null;
                if (post.post_files != null) filestr = await _fileService.uploadPostFiles(post.post_files, "Post");
                if (filestr == null && post.post_files != null) return BadRequest("Failed to Upload");

                post.post_file_urls = filestr;
                await _context.posts.AddAsync(post);
                await _context.SaveChangesAsync();
                PostsCountingInformationModel postsCountingInformationModel = 
                    new PostsCountingInformationModel
                {
                    post_id = post.post_id,
                };
                await _context.postsCountingInformation.AddAsync(postsCountingInformationModel);
                await _context.SaveChangesAsync();
                post.posts_counting_info_id = postsCountingInformationModel.posts_counting_info_id;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Successfully Posted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getAllPosts/{logedUserId}/{pageNumber}")]
        public async Task<ActionResult> getAllPosts(int logedUserId, int pageNumber = 1)
        {
            try
            {
                var query = _context.posts.Include(fr => fr.postsCountingInformation)
                .Include(fr => fr.users).Include(fr => fr.users.analytics).Include(fr => fr.groups).AsQueryable();

                var allPosts = await query
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();
                for(int i=0;i<allPosts.Count;i++)
                {
                    allPosts[i].is_liked = await _context.postLikes.AnyAsync(pl => pl.post_id == allPosts[i].post_id && pl.user_id== logedUserId);
                }


                return Ok(allPosts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("getPostByUserId/{logedUserId}/{userId}/{pageNumber}")]
        public async Task<ActionResult> getPostByUserId(int logedUserId,int userId, int pageNumber = 1)
        {
            try
            {
                var query = _context.posts.Include(fr => fr.postsCountingInformation)
                .Include(fr => fr.users).Include(fr => fr.users.analytics).Include(fr => fr.groups)
                .Where(post_info => post_info.user_id== userId).AsQueryable();


                var search_post = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();


                for (int i = 0; i < search_post.Count; i++)
                {
                    search_post[i].is_liked = await _context.postLikes.AnyAsync(pl => pl.post_id == search_post[i].post_id && pl.user_id == logedUserId);
                }


                return Ok(search_post);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }



        [HttpGet("getPostByGroupId/{logedUserId}/{groupId}/{pageNumber}")]
        public async Task<ActionResult> getPostByGroupId(int logedUserId,int groupId, int pageNumber = 1)
        {
            try
            {
                var query = _context.posts.Include(fr => fr.postsCountingInformation)
                .Include(fr => fr.users).Include(fr => fr.users.analytics)
                .Include(fr => fr.groups)
                .Where(post_info => post_info.group_id == groupId && 
                (post_info.groups.members.Any(m => m.user_id == logedUserId) ||
                post_info.groups.is_group_public==true)).AsQueryable();

                var search_post = await query
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();
                for (int i = 0; i < search_post.Count; i++)
                {
                    search_post[i].is_liked = await _context.postLikes.AnyAsync(pl => pl.post_id == search_post[i].post_id && pl.user_id == logedUserId);
                }

                return Ok(search_post);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpGet("getPostBySearch/{logedUserId}/{searchKey}/{pageNumber}")]
        public async Task<ActionResult> getPostBySearch(int logedUserId
        ,string? searchKey, int pageNumber = 1)
        {
            try
            {
                var query = _context.posts.Include(fr => fr.postsCountingInformation)
                .Include(fr => fr.users).Include(fr => fr.users.analytics).Include(fr => fr.groups)
                .Where(

                    post_info => 
                           post_info.text.ToLower().Contains(searchKey)
                        || (post_info.groups.gorup_name.ToString().ToLower().Contains(searchKey) &&
                        (post_info.groups.members.Any(pst => pst.user_id == logedUserId) ||
                            post_info.groups.is_group_public==true))
                        || post_info.users.first_name.ToString().ToLower().Contains(searchKey)
                        || post_info.users.last_name.ToString().ToLower().Contains(searchKey)
                        || post_info.users.email.ToString().ToLower().Contains(searchKey)

                        ).AsQueryable();

                var search_post = await query
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();
                for (int i = 0; i < search_post.Count; i++)
                {
                    search_post[i].is_liked = await _context.postLikes.AnyAsync(pl => pl.post_id == search_post[i].post_id && pl.user_id == logedUserId);
                }


                return Ok(search_post);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut("editPost")]
        public async Task<ActionResult> editPost([FromForm] PostModel updatedPost)
        {

            using var transaction = _context.Database.BeginTransaction();
            try
            {

                var post = await _context.posts.FindAsync(updatedPost.post_id);
                if (post != null && post.user_id== updatedPost.user_id)
                {
                    string? postFile_urls = null;
                    if (updatedPost.post_files != null)
                        postFile_urls = await _fileService.uploadPostFiles(updatedPost.post_files,"Post");
                    if (postFile_urls == null && updatedPost.post_files != null) return BadRequest("Failed to Upload");
                   

                        post.text = updatedPost.text;
                        post.place = updatedPost.place;
                        post.post_file_urls = postFile_urls;

                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok("Post is Sucessfully Updated");
                }
                else
                {
                    await transaction.RollbackAsync();
                    return NotFound("Post doesn't Exist");
                }

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("deletePost/{postId}/{userId}")]
        public async Task<ActionResult> deletePost(int postId,int userId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Task task1 = _context.posts.Where(p => p.post_id == postId && p.user_id == userId).ExecuteDeleteAsync();
                Task task2 = _context.postsCountingInformation.Where(countTable => countTable.post_id == postId).ExecuteDeleteAsync();
                Task task3 = _context.postComments.Where(commentsTable => commentsTable.post_id ==postId).ExecuteDeleteAsync();
                Task task4 = _context.postLikes.Where(likesTable => likesTable.post_id == postId).ExecuteDeleteAsync();

                await Task.WhenAll(task1, task2, task3, task4);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Post is Sucessfully Deleted");

            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
            
        }


     
    }
}
