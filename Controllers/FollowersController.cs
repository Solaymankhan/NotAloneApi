using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class FollowersController : ControllerBase
    {
        int pageSize = 20;
        ApplicationDbContext _context;

        public FollowersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postFollower")]
        public async Task<ActionResult> postFollower([FromForm] FollowersModel follower)
        {
            try
            {
                _context.followers.Add(follower);
                await _context.SaveChangesAsync();
                Task task1 = incrementDecrementFollower(follower.follower_user_id, "increment", "follower");
                Task task2 = incrementDecrementFollower(follower.following_user_id, "increment", "following");
                await Task.WhenAll(task1, task2);

                await _context.SaveChangesAsync();
                return Ok("Followed Successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }          
        }

        [HttpGet("getFollowers/{user_id}/{pageNumber}")]
        public async Task<ActionResult> getFollowers(int user_id, int pageNumber = 1)
        {

            try
            {
                var query = _context.followers
                    .Include(fr => fr.follower_user)
                   .Where(fr => fr.following_user_id == user_id)
                   .AsQueryable();

                var followers = await query
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

                return Ok(followers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getFollowings/{user_id}/{pageNumber}")]
        public async Task<ActionResult> getFollowings(int user_id, int pageNumber = 1)
        {
            try
            {
                var query =  _context.followers
                   .Include(fr => fr.following_user)
                   .Where(fr => fr.follower_user_id == user_id)
                   .AsQueryable();

                var followings = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

                 return Ok(followings);
             
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteFollowings/{follower_user_id}/{following_user_id}")]
        public async Task<ActionResult> deleteFollowings(int follower_user_id,int following_user_id)
        {
            try
            {
                await _context.followers
                   .Where(fr => fr.follower_user_id == follower_user_id
                   && fr.following_user_id == following_user_id).ExecuteDeleteAsync();
                Task task1 = incrementDecrementFollower(follower_user_id, "decrement", "follower");
                Task task2 = incrementDecrementFollower(following_user_id, "decrement", "following");
                await Task.WhenAll(task1, task2);

                await _context.SaveChangesAsync();

                return Ok("Unfollowed Sucessfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task incrementDecrementFollower(int? user_id
            , string increment_decrement, string following_follower)
        {
            if (increment_decrement == "increment" && following_follower == "following")
                _context.usersCounting
                  .Where(val => val.user_id == user_id)
                  .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_followers, pci => pci.total_followers+1));
        
            else if (increment_decrement == "increment" && following_follower == "follower")
                _context.usersCounting
                  .Where(val => val.user_id == user_id)
                  .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_followings, pci => pci.total_followings + 1));
            else if (increment_decrement == "decrement" && following_follower == "following")
                _context.usersCounting
                  .Where(val => val.user_id == user_id)
                  .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_followers, pci => pci.total_followers- 1));
            else if (increment_decrement == "decrement" && following_follower == "follower")
                _context.usersCounting
                  .Where(val => val.user_id == user_id)
                  .ExecuteUpdate(pci => pci.SetProperty(pci => pci.total_followings, pci => pci.total_followings-1));

        }


    }
}
