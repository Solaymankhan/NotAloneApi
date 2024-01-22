using NotAlone.Data;
using NotAlone.Models;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class FriendRequstsController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;
        public FriendRequstsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("postFriendRequest")]
        public async Task<ActionResult> postFriendRequest([FromForm] FriendRequestsModel friendRequest)
        {
            try
            {
                await _context.friendRequests.AddAsync(friendRequest);

                _context.analytics.Where(val => val.user_id == friendRequest.reciever_id)
                  .ExecuteUpdate(user => user.SetProperty(user => user.total_new_friend_requests,
                  user => user.total_new_friend_requests+1));
                await _context.SaveChangesAsync();
                return Ok("Friend Requst is Successfully sent");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }

        [HttpGet("getFriendRequestSended/{user_id}/{pageNumber}")]
        public async Task<ActionResult> getFriendRequestSended(int user_id, int pageNumber = 1)
        {

            try
            {
                var query = _context.friendRequests
                  .Include(fr => fr.reciever)
                  .Where(fr => fr.sender_id == user_id)
                  .AsQueryable();

                var friendRequests = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

                return Ok(friendRequests);
                

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpGet("getFriendRequestRecived/{user_id}/{pageNumber}")]
        public async Task<ActionResult> getFriendRequestRecieved(int user_id, int pageNumber = 1)
        {

            try
            {
                var query = _context.friendRequests
                  .Include(fr => fr.sender)
                  .Where(fr => fr.reciever_id == user_id)
                  .AsQueryable();

                var friendRequests = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

                return Ok(friendRequests);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("friendRequestStatus/{user1}/{user2}")]
        public async Task<ActionResult> friendRequestStatus(int user1, int user2)
        {
            List<string> badresponse = ["false"];
            try
            {
                FriendRequestsModel? requests1 =
                    await _context.friendRequests
                    .Where(val => val.sender_id == user1 && val.reciever_id == user2)
                    .FirstOrDefaultAsync();
                FriendRequestsModel? requests2 =
                    await _context.friendRequests
                    .Where(val => val.sender_id == user2 && val.reciever_id == user1)
                    .FirstOrDefaultAsync();

                //request_status : true/false

                if (requests1 != null)
                {
                    // response     : [true/false,sender]
                    List<string> response = [requests1.request_status.ToString(), "sender"];
                    return Ok(response);
                }
                else if (requests2 != null)
                {
                    //  response     : [true/false,reciever]
                    List<string> response = [requests2.request_status.ToString()!, "reciever"];
                    return Ok(response);
                }
                else return NotFound(badresponse);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("acceptFriendRequest/{sender_id}/{reciever_id}")]
        public async Task<ActionResult> acceptFriendRequest(int sender_id, int reciever_id)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {

                await _context.friends.AddAsync(new FriendsModel
                {
                    user1_id = sender_id,
                    user2_id = reciever_id,
                });

                Task task1 = _context.friendRequests
                .Where(val => val.sender_id == reciever_id && val.reciever_id == sender_id)
                .ExecuteUpdateAsync(request => request.SetProperty(request => request.request_status,true));
                Task task2 = incrementTotalFriends(sender_id);
                Task task3 = incrementTotalFriends(reciever_id);
                await Task.WhenAll(task1, task2,task3);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Friend request is Sucessfully Accepted");

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(e.Message);
            }
        }

        private async Task incrementTotalFriends(int? userId)
        {
            await _context.usersCounting
                       .Where(val => val.user_id == userId)
                  .ExecuteUpdateAsync(pci => pci.SetProperty(uc => uc.total_friends, uc => uc.total_friends + 1));

        }

        [HttpDelete("deleteFriendRequest/{sender_id}/{reciever_id}")]
        public async Task<ActionResult> deleteFriendRequest(int sender_id, int reciever_id)
        {
            try
            {
                await _context.friendRequests
                    .Where(val => (val.sender_id == sender_id && val.reciever_id == reciever_id)
                    ||(val.sender_id == reciever_id && val.reciever_id == sender_id)).ExecuteDeleteAsync();

                return Ok("Friend Request is Deleted Successfully");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
