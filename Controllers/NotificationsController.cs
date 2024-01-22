using NotAlone.Data;
using NotAlone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class NotificationsController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context; 
        private readonly IHubContext<MessageHub> _messageConversationHub;


        public NotificationsController(IHubContext<MessageHub> messageConversationHub, 
            ApplicationDbContext context)
        {
            _messageConversationHub = messageConversationHub;
            _context = context;
        }

        [HttpPost("postNotifications")]
        public async Task<ActionResult> postNotifications([FromForm] NotificationModel postnotification)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.notifications.AddAsync(postnotification);

                _context.analytics
                 .Where(anltc => anltc.user_id== postnotification.to_user_id)
                 .ExecuteUpdate(cci => cci.SetProperty(cci => cci.total_new_notifications, cci => cci.total_new_notifications + 1));

                await _messageConversationHub.Clients
                   .User(postnotification.to_user_id.ToString()!)
                   .SendAsync("notificationSend", "New Notification send!");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Notification Successfully Posted");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete("deleteNotification/{notificationId}/{userId}")]
        public async Task<ActionResult> deleteNotification(int notificationId, int userId)
        {
            try
            {
                await _context.notifications
                    .Where(notificationTable => notificationTable.to_user_id == userId
                    && notificationTable.notification_id == notificationId).ExecuteDeleteAsync();

                return Ok("Notification is Successfully Deleted");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("readNotification/{notificationId}/{userId}")]
        public async Task<ActionResult> readNotification(int notificationId, int userId)
        {
            try
            {
                await _context.notifications
                    .Where(notificationTable => notificationTable.to_user_id == userId
                    && notificationTable.notification_id == notificationId)
                    .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.is_read, true));

                return Ok("Notification is Successfully Read");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("seenNotification/{userId}")]
        public async Task<ActionResult> seenNotification( int userId)
        {
           
            try
            {
                await _context.analytics
                 .Where(anltc => anltc.user_id == userId)
                 .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.total_new_notifications, 0));
                return Ok("Notification is Successfully seen");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("getNotifications/{userId}/{pageNumber}")]
        public async Task<ActionResult> getNotifications(int userId, int pageNumber = 1)
        {
            try
            {
                var query = _context.notifications
                    .Include(notificationTable => notificationTable.fromUser)
                    .Where(notificationTable => notificationTable.to_user_id == userId).AsQueryable();

                var notifications = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(notifications);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
