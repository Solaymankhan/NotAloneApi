using NotAlone.Data;
using NotAlone.Models;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class FriendsController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;
        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("getFriends/{user_id}/{pageNumber}")]
        public async Task<ActionResult> getFriends(int user_id, int pageNumber = 1)
        {

            try
            {
                var query = _context.friends
                  .Include(fr => fr.user1)
                  .Include(fr => fr.user2)
                  .Where(fr => fr.user1_id == user_id || fr.user2_id == user_id)
                  .AsQueryable();

                var friends = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

                return Ok(friends);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("deleteFriend/{user1_id}/{user2_id}")]
        public async Task<ActionResult> deleteFriend(int user1_id,int user2_id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Task task1 = _context.friends
                  .Where(fr => (fr.user1_id == user1_id && fr.user2_id == user2_id) 
                  ||(fr.user1_id == user2_id && fr.user2_id == user1_id)).ExecuteDeleteAsync();
                Task task2 = _context.friendRequests
                   .Where(val => (val.sender_id == user1_id && val.reciever_id == user2_id)
                   || (val.sender_id == user2_id && val.reciever_id == user1_id)).ExecuteDeleteAsync();
                Task task3 = decrementTotalFriends(user1_id);
                Task task4 = decrementTotalFriends(user2_id);
                await Task.WhenAll(task1, task2, task3, task4);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Unfriended Successfully");

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }

        private async Task decrementTotalFriends(int? userId)
        {

            _context.usersCounting
                  .Where(val => val.user_id == userId)
                  .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_friends, pci => pci.total_friends - 1));
        }

    }
}
