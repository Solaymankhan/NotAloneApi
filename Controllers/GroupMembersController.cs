using NotAlone.Data;
using NotAlone.Models;
using NotAlone.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class GroupMembersController : ControllerBase
    {
        int pageSize = 20;
        private readonly ApplicationDbContext _context;

        public GroupMembersController( ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postGroupMembers")]
        public async Task<ActionResult> postGroupMembers([FromForm] GroupMembersModel groupMembers)
        {
            try
            {
                _context.groupMembers.AddAsync(groupMembers);
                await _context.SaveChangesAsync();
                return Ok("Member is Sucessfully Posted");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut("acceptGroupMembership/{grpMembershipId}/{groupId}/{adminId}")]
        public async Task<ActionResult> acceptGroupMembership(int grpMembershipId, int groupId, int adminId)
        {

            try
            {
                
                Task task1= _context.groupMembers
                .Where(u => u.group_membership_id== grpMembershipId)
                .ExecuteUpdateAsync(u=>u.SetProperty(u=>u.membership_status,"Joined"));

                Task task2 = _context.groups
                .Where(u => u.group_id == groupId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.total_members, u => u.total_members+1));
                await Task.WhenAll(task1, task2);
                await _context.SaveChangesAsync();

                return Ok("Membership Accepted");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpDelete("deleteGroupMembership/{grpMembershipId}/{groupId}")]
        public async Task<ActionResult> deleteGroupMembership(int grpMembershipId, int groupId)
        {

            try
            {

                var grpMembership = await _context.groupMembers.FindAsync(grpMembershipId);

                if(grpMembership==null) return BadRequest("Membership doesn't found");

                if(grpMembership.membership_status=="Joined")
                    _context.groups
                .Where(u => u.group_id == groupId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.total_members, u => u.total_members - 1));

                _context.groupMembers.Remove(grpMembership);
                
                await _context.SaveChangesAsync();
                return Ok("Membership Deleted");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("getGroupMembers/{grpId}/{pageNumber}")]
        public async Task<ActionResult> getGroupMembers(int grpId, int pageNumber = 1)
        {

            try
            {
                var query = _context.groupMembers
                    .Include(grp=>grp.user)
                    .Where(grp=>grp.group_id==grpId).AsQueryable();


                var groupmembers = await query
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToListAsync();


                return Ok(groupmembers);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
