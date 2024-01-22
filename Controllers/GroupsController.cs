using NotAlone.Data;
using NotAlone.Models;
using NotAlone.Services.Abstract;
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
    public class GroupsController : ControllerBase
    {
        int pageSize = 20;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public GroupsController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }


        [HttpPost("postGroups")]
        public async Task<ActionResult> postGroups([FromForm] GroupsModel groups)
        {

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                string? grpImgstr = null, grpBgImgstr = null;
                if (groups.group_img_file != null)
                    grpImgstr = await _fileService.uploadSingleFile(groups.group_img_file, "profileimages");
                if (grpImgstr == null && groups.group_img_file != null) return BadRequest("Failed to Upload");
                if (groups.group_bg_img_file != null)
                    grpBgImgstr = await _fileService.uploadSingleFile(groups.group_bg_img_file, "backgroundimages");
                if (grpBgImgstr == null && groups.group_bg_img_file != null) return BadRequest("Failed to Upload");

                groups.group_img_url = grpImgstr;
                groups.group_bg_img_url = grpBgImgstr;

                await _context.groups.AddAsync(groups);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Group is Successfully added.");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }

        [HttpPut("editGroup")]
        public async Task<ActionResult> editGroup([FromForm] GroupsModel groups)
        {

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var grp = await _context.groups.FindAsync(groups.group_id);

                string? grpImgstr = null, grpBgImgstr = null;
                if (groups.group_img_file != null)
                    grpImgstr = await _fileService.uploadSingleFile(groups.group_img_file, "profileimages");
                if (grpImgstr == null && groups.group_img_file != null) return BadRequest("Failed to Upload");
                if (groups.group_bg_img_file != null)
                    grpBgImgstr = await _fileService.uploadSingleFile(groups.group_bg_img_file, "backgroundimages");
                if (grpBgImgstr == null && groups.group_bg_img_file != null) return BadRequest("Failed to Upload");

                grp.group_img_url = grpImgstr;
                grp.group_bg_img_url = grpBgImgstr;
                grp.gorup_name = groups.gorup_name;
                grp.gorup_description_txt = groups.gorup_description_txt;
                grp.is_group_public = groups.is_group_public;
                await _context.SaveChangesAsync();

                _context.groupMembers.AddAsync(new GroupMembersModel { 
                    group_id=grp.group_id,
                    user_id=grp.group_admin_id,
                    membership_status= "Joined"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Group is Successfully Edited.");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("deleteGroup/{groupId}/{adminId}")]
        public async Task<ActionResult<IEnumerable<UserModel>>> deleteGroup(int groupId, int adminId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Task task1 = _context.groups.Where(usr => usr.group_admin_id == adminId && usr.group_id == groupId).ExecuteDeleteAsync();
                Task task2 = _context.groupMembers
                   .Where(val => val.group_id == groupId).ExecuteDeleteAsync();
                Task task3 = _context.posts
               .Where(val => val.group_id == groupId).ExecuteDeleteAsync();

                await Task.WhenAll(task1, task2, task3);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Group is Sucessfully Deleted");

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }

        }





        [HttpGet("getAllGroups/{pageNumber}")]
        public async Task<ActionResult> getAllGroups(int pageNumber = 1)
        {

            try
            {
                 var query = _context.groups.AsQueryable();

                var groups = await query
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToListAsync();


                return Ok(groups);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getGroupsByUserId/{userId}/{pageNumber}")]
        public async Task<ActionResult> getGroupsByUserId(int userId, int pageNumber = 1)
        {

            try
            {
                var query = _context.groups.Where(g => g.members.Any(m => m.user_id == userId))
                    .AsQueryable();

                var groups = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();


                return Ok(groups);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }




    }
}
