
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotAlone.Models;
using NotAlone.Services.Implementation;

namespace NotAlone.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
      
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option):base(option)
        {

        }
        public DbSet<UserModel> users { get; set; }
        public DbSet<UsersCountingInformationModel> usersCounting { get; set; }
        public DbSet<FriendRequestsModel> friendRequests { get; set; }
        public DbSet<FriendsModel> friends { get; set; }
        public DbSet<FollowersModel> followers { get; set; }
        public DbSet<PostModel> posts { get; set; }
        public DbSet<PostsCountingInformationModel> postsCountingInformation { get; set; }
        public DbSet<PostLikesModel> postLikes { get; set; }
        public DbSet<PostCommentsModel> postComments { get; set; }
        public DbSet<CommentsCountingInformationModel> commentsCountingInformation { get; set; }
        public DbSet<CommentsLikesModel> commentLikes { get; set; }
        public DbSet<CommentRepliesModel> commentReplies { get; set; }
        public DbSet<CommentRepliesLikesModel> commentRepliesLikes { get; set; }
        
        public DbSet<NotificationModel> notifications { get; set; }
        public DbSet<MessageModel> messages { get; set; }
        public DbSet<DeleteLastMessageUserModel> deleteLastMessageUsers { get; set; }
        public DbSet<MessageConversationModel> messageConversations { get; set; }
        public DbSet<AnalyticsModel> analytics { get; set; }
        public DbSet<ReportsModel> reports { get; set; }
        public DbSet<GroupsModel> groups { get; set; }
        public DbSet<GroupMembersModel> groupMembers { get; set; }

    }
}
