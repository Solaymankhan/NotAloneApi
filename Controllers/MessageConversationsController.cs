using NotAlone.Data;
using NotAlone.Models;
using NotAlone.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotAlone.Controllers
{
    [Route("notAlone/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class MessageConversationsController : ControllerBase
    {
        int pageSize = 20;
        private ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> _messageConversationHub;
        private readonly IFileService _fileService;

        public MessageConversationsController(IHubContext<MessageHub> messageConversationHub, 
            IFileService fileService, ApplicationDbContext context)
        {
            _messageConversationHub = messageConversationHub;
            _fileService = fileService;
            _context = context;
        }


        [HttpPost("postMessages")]
        public async Task<ActionResult> postMessages([FromForm] MessageConversationModel messagesConversation)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                string? filestr = null;
                if (messagesConversation.message_files != null)
                    filestr = await _fileService.uploadPostFiles(messagesConversation.message_files, "Message");
                if (filestr == null && messagesConversation.message_files != null) return BadRequest("Failed to Upload");

                messagesConversation.message_files_url = filestr;

                await _context.messageConversations.AddAsync(messagesConversation);
                MessageModel? lastMessage=null;
                if (messagesConversation.group_id == null)
                {
                    lastMessage = await _context.messages
                        .Where(msg => (msg.user1_id == messagesConversation.sender_id 
                        && msg.user2_id == messagesConversation.reciever_id) 
                        || (msg.user2_id == messagesConversation.sender_id &&
                        msg.user1_id == messagesConversation.reciever_id)).FirstOrDefaultAsync();
                }
                else
                {
                    lastMessage = await _context.messages
                        .Where(msg => msg.user2_id== messagesConversation.group_id 
                        && msg.group_id == messagesConversation.group_id).FirstOrDefaultAsync();
                }

                if (lastMessage == null)
                {
                    _context.messages.Add(new MessageModel
                    {   
                        user1_id = messagesConversation.group_id != null ? null : messagesConversation.sender_id,
                        user2_id = messagesConversation.group_id != null ? null:messagesConversation.reciever_id,
                        sender_id= messagesConversation.sender_id,
                        is_group_message=messagesConversation.group_id!=null?true:false,
                        group_id= messagesConversation.group_id,
                        is_read = false,
                        isText = filestr == null ? true : false,
                        last_txt = filestr == null ? messagesConversation.message_txt : null,
                    });
                }

                else if (lastMessage != null)
                {

                    lastMessage.isText = filestr == null ? true : false;
                    lastMessage.last_txt = filestr == null ? messagesConversation.message_txt : null;
                    lastMessage.sender_id = messagesConversation.sender_id;
                    lastMessage.last_messages_time = DateTime.Now;
                    lastMessage.is_read = false;
                    lastMessage.new_message_arrive = true;
                }

                _context.analytics
                 .Where(cci => cci.user_id == messagesConversation.reciever_id)
                 .ExecuteUpdate(cci => cci.SetProperty(cci => cci.total_new_messages, cci => cci.total_new_messages + 1));


                if (messagesConversation.group_id != null)
                {
                    // Notify the group members about the new message
                    await _messageConversationHub.Clients.Group($"Group_{messagesConversation.group_id}")
                        .SendAsync("ReceiveGroupMessage", "New group message received!");

                }
                else
                {
                    // Notify the sender about the new message
                    await _messageConversationHub.Clients
                    .User(messagesConversation.reciever_id.ToString()!)
                    .SendAsync("ReceiveSend", "New message received!");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Message sent Successfully");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("getSingleUserMessages/{lastMessageId}/{senderId}/{recieverId}/{pageNumber}")]
        public async Task<ActionResult> getSinglUsereMessages(int lastMessageId,int senderId, int recieverId, int pageNumber = 1)
        {
            try
            {
               var lastMessage= await _context.messages.FindAsync(lastMessageId);
                lastMessage!.is_read = true;
                await _context.SaveChangesAsync();
                DateTime? delete_time = lastMessage.deleted_last_message_users!
                    .FirstOrDefault(e => e.user_id == senderId)!.delete_time;


                var query = _context.messageConversations
                    .OrderByDescending(q=>q.messages_time)
                    .Include(conversationTable => conversationTable.user)
                    .Include(conversationTable => conversationTable.user!.analytics)
                    .Where(conversationTable => 
                    ((conversationTable.sender_id == senderId && conversationTable.reciever_id == recieverId)
                    ||(conversationTable.sender_id == recieverId && conversationTable.reciever_id == senderId))
                    && conversationTable.delete_onlyFor_user_id!=senderId 
                    && (delete_time == null ? true : delete_time <= conversationTable.messages_time)).AsQueryable();


                var msgs = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Notify the sender about the new message
                await _messageConversationHub.Clients
                   .User(senderId.ToString())
                   .SendAsync("ReceiveMessage", "New message received!");

                return Ok(msgs);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("getGroupMessages/{lastMessageId}/{senderId}/{groupId}/{pageNumber}")]
        public async Task<ActionResult> getGroupMessages(int lastMessageId, int senderId, int groupId, int pageNumber = 1)
        {
            try
            {
                var lastMessage = await _context.messages.FindAsync(lastMessageId);
                lastMessage!.is_read = true;
                await _context.SaveChangesAsync();
                DateTime? delete_time =lastMessage.deleted_last_message_users!
                    .FirstOrDefault(e => e.user_id == senderId)!.delete_time;

                var query = _context.messageConversations
                      .Include(conversationTable => conversationTable.user)
                      .Include(conversationTable => conversationTable.user!.analytics)
                      .Where(conversationTable => conversationTable.group_id == groupId
                             && delete_time == null ? true : delete_time <= conversationTable.messages_time ).AsQueryable();

                var messages = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                await _messageConversationHub.Clients.Group($"Group_{groupId}")
                    .SendAsync("ReceiveGroupMessage", "New group message received!");

                return Ok(messages);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("deleteMessagesOnlyForSingleUser/{messageId}/{user1Id}")]
        public async Task<ActionResult> deleteMessagesOnlyForSingleUser(int messageId, int user1Id)
        {
            try
            {
                await _context.messageConversations
                    .Where(conversationsTable => conversationsTable.messages_id == messageId)
                    .ExecuteUpdateAsync(cci => cci.SetProperty(cci => cci.delete_onlyFor_user_id, user1Id));

                await _messageConversationHub.Clients
                  .User(user1Id.ToString())
                  .SendAsync("ReceiveMessage", "New message received!");

                await _context.SaveChangesAsync();
                return Ok("Message Conversation Successfully Deleted only for you");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpDelete("deleteMessage/{conversationId}/{user1Id}/{user2Id}")]
        public async Task<ActionResult> deleteMessages(int conversationId, int user1Id, int user2Id)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.messageConversations
                    .Where(conversationsTable => conversationsTable.messages_id == conversationId).ExecuteDeleteAsync();

                var conversation = await _context.messageConversations.Where(conversationTable => (conversationTable.sender_id == user1Id && conversationTable.reciever_id == user2Id) || (conversationTable.reciever_id == user2Id &&
                    conversationTable.sender_id == user1Id)).FirstOrDefaultAsync();

                if (conversation == null)
                   await _context.messages.Where(msg => (msg.user1_id == user1Id && msg.user2_id == user2Id) || (msg.user1_id        == user2Id && msg.user2_id == user1Id)).ExecuteDeleteAsync();
             


                await _messageConversationHub.Clients
                 .User(user1Id.ToString())
                 .SendAsync("ReceiveMessage", "New message received!");
                await _messageConversationHub.Clients
                 .User(user2Id.ToString())
                 .SendAsync("ReceiveMessage", "New message received!");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Message Conversation Successfully Deleted");

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }

        }


    }
}
