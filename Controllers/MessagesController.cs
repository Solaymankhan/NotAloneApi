using NotAlone.Data;
using NotAlone.Models;
using NotAlone.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class MessagesController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> _messageConversationHub;

        public MessagesController(IHubContext<MessageHub> messageConversationHub,ApplicationDbContext context)
        {
            _messageConversationHub = messageConversationHub;
            _context = context;
        }



        [HttpGet("getLastMessage/{userId}/{pageNumber}")]
        public async Task<ActionResult> getLastMessages(int userId ,int pageNumber = 1)
        {
            try
            {
                var query =  _context.messages
                    .OrderBy(msg=>msg.last_messages_time)
                    .Where(
                    messageTable => (
                       messageTable.user1_id == userId
                    || messageTable.user2_id == userId 
                    || messageTable.groups!.members!.Any(grp => grp.user_id == userId))
                    && (messageTable.deleted_last_message_users!.Any(person=>person.user_id!=userId)
                    || messageTable.new_message_arrive!=false)).AsQueryable();

                var lastMessages = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                await _messageConversationHub.Clients
                 .User(userId.ToString())
                 .SendAsync("ReceiveMessage", "Message List updated!");

                return Ok(lastMessages);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpDelete("deleteLastMessage/{lastMessageId}/{user1Id}/{user2Id}")]
        public async Task<ActionResult> deleteLastMessages(int lastMessageId, int user1Id, int user2Id)
        {
            try
            {
                var lastMessage = await _context.messages.FindAsync(lastMessageId);
                if (lastMessage == null) return BadRequest("This Message doesn't exixt");
                if (lastMessage.deleted_last_message_users!.Any(person => person.message_id == lastMessageId && person.user_id == user2Id) && lastMessage.group_id==null)
                {
                    _context.deleteLastMessageUsers.Where(lastmsg => lastmsg.message_id == lastMessageId).ExecuteDelete();
                    _context.messages.Remove(lastMessage);
                    _context.messageConversations
                    .Where(messageTable => (messageTable.sender_id == user1Id && messageTable.reciever_id == user2Id)
                    || (messageTable.sender_id == user2Id && messageTable.reciever_id == user1Id)).ExecuteDelete();

                }
                else if (lastMessage.deleted_last_message_users!.Any(person => person.message_id == lastMessageId && person.user_id == user1Id) && lastMessage.group_id == null)
                {
                    _context.deleteLastMessageUsers.Where(dltmsg=>dltmsg.message_id==lastMessageId).FirstOrDefault()!.delete_time = DateTime.Now;
                    _context.messages.Find(lastMessage)!.new_message_arrive = false;
                    await _context.SaveChangesAsync();

                }
                else if (lastMessage.deleted_last_message_users!.Any(person => person.message_id == lastMessageId) && lastMessage.group_id != null
                    && lastMessage.deleted_last_message_users!.Count >= _context.groups.FindAsync(lastMessage.group_id).Result!.members!.Count-1 )
                {
                    _context.deleteLastMessageUsers.Where(lastmsg=>lastmsg.message_id==lastMessageId).ExecuteDelete();
                    _context.messages.Remove(lastMessage);
                    _context.messageConversations
                    .Where(messageTable => messageTable.group_id == lastMessage.group_id).ExecuteDelete();

                }
                else
                {
                   _context.deleteLastMessageUsers.Add(new DeleteLastMessageUserModel
                    {
                        message_id=lastMessageId,
                        user_id=user1Id,
                    });
                    _context.messages.Find(lastMessage)!.new_message_arrive = false;
                    await _context.SaveChangesAsync();
                }

                if(lastMessage.group_id == null)
                     await _messageConversationHub.Clients
                          .User(user1Id.ToString())
                          .SendAsync("ReceiveMessage", "Message deleted!");
                else
                    await _messageConversationHub.Clients.Group($"Group_{lastMessage.group_id}")
                        .SendAsync("ReceiveGroupMessage", "New group message received!");

                return Ok("Last Message Successfully Deleted");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



    }
}
