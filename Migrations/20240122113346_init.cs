using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotAlone.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analytics",
                columns: table => new
                {
                    analytics_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    hash_tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    analytics_user_ids = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    analytics_group_ids = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_new_notifications = table.Column<int>(type: "int", nullable: true),
                    total_new_messages = table.Column<int>(type: "int", nullable: true),
                    total_new_reports = table.Column<int>(type: "int", nullable: true),
                    total_new_friend_requests = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    last_active_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analytics", x => x.analytics_id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "commentsCountingInformation",
                columns: table => new
                {
                    comments_counting_info_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comments_id = table.Column<int>(type: "int", nullable: true),
                    total_likes = table.Column<int>(type: "int", nullable: true),
                    total_replies = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commentsCountingInformation", x => x.comments_counting_info_id);
                });

            migrationBuilder.CreateTable(
                name: "postsCountingInformation",
                columns: table => new
                {
                    posts_counting_info_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    total_likes = table.Column<int>(type: "int", nullable: true),
                    total_comments = table.Column<int>(type: "int", nullable: true),
                    total_shares = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postsCountingInformation", x => x.posts_counting_info_id);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reportedTo_user_id = table.Column<int>(type: "int", nullable: true),
                    reportedBy_user_id = table.Column<int>(type: "int", nullable: true),
                    reported_item_id = table.Column<int>(type: "int", nullable: true),
                    report_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    report_description_txt = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    report_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.report_id);
                });

            migrationBuilder.CreateTable(
                name: "usersCounting",
                columns: table => new
                {
                    usersCountingInformation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    total_friends = table.Column<int>(type: "int", nullable: true),
                    total_followers = table.Column<int>(type: "int", nullable: true),
                    total_followings = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usersCounting", x => x.usersCountingInformation_id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    profile_picture_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    baground_picture_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    user_registration_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    analytics_id = table.Column<int>(type: "int", nullable: true),
                    usersCountingInformation_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_analytics_analytics_id",
                        column: x => x.analytics_id,
                        principalTable: "analytics",
                        principalColumn: "analytics_id");
                    table.ForeignKey(
                        name: "FK_users_usersCounting_usersCountingInformation_id",
                        column: x => x.usersCountingInformation_id,
                        principalTable: "usersCounting",
                        principalColumn: "usersCountingInformation_id");
                });

            migrationBuilder.CreateTable(
                name: "commentLikes",
                columns: table => new
                {
                    liked_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    comment_id = table.Column<int>(type: "int", nullable: true),
                    liked_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commentLikes", x => x.liked_id);
                    table.ForeignKey(
                        name: "FK_commentLikes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "commentReplies",
                columns: table => new
                {
                    replied_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    comment_id = table.Column<int>(type: "int", nullable: true),
                    replied_txt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    total_likes = table.Column<int>(type: "int", nullable: true),
                    repleid_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commentReplies", x => x.replied_id);
                    table.ForeignKey(
                        name: "FK_commentReplies_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "commentRepliesLikes",
                columns: table => new
                {
                    comment_replies_liked_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    comment_reply_id = table.Column<int>(type: "int", nullable: true),
                    liked_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commentRepliesLikes", x => x.comment_replies_liked_id);
                    table.ForeignKey(
                        name: "FK_commentRepliesLikes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "followers",
                columns: table => new
                {
                    followers_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    follower_user_id = table.Column<int>(type: "int", nullable: true),
                    following_user_id = table.Column<int>(type: "int", nullable: true),
                    following_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_followers", x => x.followers_id);
                    table.ForeignKey(
                        name: "FK_followers_users_follower_user_id",
                        column: x => x.follower_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_followers_users_following_user_id",
                        column: x => x.following_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "friendRequests",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_id = table.Column<int>(type: "int", nullable: true),
                    reciever_id = table.Column<int>(type: "int", nullable: true),
                    request_status = table.Column<bool>(type: "bit", nullable: true),
                    request_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendRequests", x => x.request_id);
                    table.ForeignKey(
                        name: "FK_friendRequests_users_reciever_id",
                        column: x => x.reciever_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_friendRequests_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    friendship_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user1_id = table.Column<int>(type: "int", nullable: true),
                    user2_id = table.Column<int>(type: "int", nullable: true),
                    friendship_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friends", x => x.friendship_id);
                    table.ForeignKey(
                        name: "FK_friends_users_user1_id",
                        column: x => x.user1_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_friends_users_user2_id",
                        column: x => x.user2_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_admin_id = table.Column<int>(type: "int", nullable: true),
                    is_group_public = table.Column<bool>(type: "bit", nullable: true),
                    gorup_description_txt = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    group_img_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    group_bg_img_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gorup_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    total_members = table.Column<int>(type: "int", nullable: true),
                    group_creation_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.group_id);
                    table.ForeignKey(
                        name: "FK_groups_users_group_admin_id",
                        column: x => x.group_admin_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "messageConversations",
                columns: table => new
                {
                    messages_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_id = table.Column<int>(type: "int", nullable: true),
                    reciever_id = table.Column<int>(type: "int", nullable: true),
                    delete_onlyFor_user_id = table.Column<int>(type: "int", nullable: true),
                    is_seen = table.Column<bool>(type: "bit", nullable: true),
                    message_txt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    message_files_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messages_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageConversations", x => x.messages_id);
                    table.ForeignKey(
                        name: "FK_messageConversations_users_reciever_id",
                        column: x => x.reciever_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    to_user_id = table.Column<int>(type: "int", nullable: true),
                    from_user_id = table.Column<int>(type: "int", nullable: true),
                    source_id = table.Column<int>(type: "int", nullable: false),
                    notification_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true),
                    notification_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_notifications_users_from_user_id",
                        column: x => x.from_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "postComments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    comment_txt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    comment_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postComments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_postComments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "postLikes",
                columns: table => new
                {
                    likes_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    liked_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postLikes", x => x.likes_id);
                    table.ForeignKey(
                        name: "FK_postLikes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "groupMembers",
                columns: table => new
                {
                    group_membership_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    membership_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    membership_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupMembers", x => x.group_membership_id);
                    table.ForeignKey(
                        name: "FK_groupMembers_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK_groupMembers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user1_id = table.Column<int>(type: "int", nullable: false),
                    user2_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: true),
                    isText = table.Column<bool>(type: "bit", nullable: true),
                    last_txt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true),
                    is_group_message = table.Column<bool>(type: "bit", nullable: true),
                    new_message_arrive = table.Column<bool>(type: "bit", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    last_messages_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_messages_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK_messages_users_user1_id",
                        column: x => x.user1_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_messages_users_user2_id",
                        column: x => x.user2_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    post_file_urls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    place = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_group_post = table.Column<bool>(type: "bit", nullable: true),
                    is_shared_post = table.Column<bool>(type: "bit", nullable: true),
                    shared_post_id = table.Column<int>(type: "int", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    posts_counting_info_id = table.Column<int>(type: "int", nullable: true),
                    post_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_posts_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK_posts_postsCountingInformation_posts_counting_info_id",
                        column: x => x.posts_counting_info_id,
                        principalTable: "postsCountingInformation",
                        principalColumn: "posts_counting_info_id");
                    table.ForeignKey(
                        name: "FK_posts_posts_shared_post_id",
                        column: x => x.shared_post_id,
                        principalTable: "posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "FK_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "deleteLastMessageUsers",
                columns: table => new
                {
                    delete_last_message_userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    delete_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deleteLastMessageUsers", x => x.delete_last_message_userId);
                    table.ForeignKey(
                        name: "FK_deleteLastMessageUsers_messages_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "message_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_commentLikes_user_id",
                table: "commentLikes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_commentReplies_user_id",
                table: "commentReplies",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_commentRepliesLikes_user_id",
                table: "commentRepliesLikes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_deleteLastMessageUsers_message_id",
                table: "deleteLastMessageUsers",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_followers_follower_user_id",
                table: "followers",
                column: "follower_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_followers_following_user_id",
                table: "followers",
                column: "following_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendRequests_reciever_id",
                table: "friendRequests",
                column: "reciever_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendRequests_sender_id",
                table: "friendRequests",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_friends_user1_id",
                table: "friends",
                column: "user1_id");

            migrationBuilder.CreateIndex(
                name: "IX_friends_user2_id",
                table: "friends",
                column: "user2_id");

            migrationBuilder.CreateIndex(
                name: "IX_groupMembers_group_id",
                table: "groupMembers",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_groupMembers_user_id",
                table: "groupMembers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_group_admin_id",
                table: "groups",
                column: "group_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_messageConversations_reciever_id",
                table: "messageConversations",
                column: "reciever_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_group_id",
                table: "messages",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_user1_id",
                table: "messages",
                column: "user1_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_user2_id",
                table: "messages",
                column: "user2_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_from_user_id",
                table: "notifications",
                column: "from_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_postComments_user_id",
                table: "postComments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_postLikes_user_id",
                table: "postLikes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_group_id",
                table: "posts",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_posts_counting_info_id",
                table: "posts",
                column: "posts_counting_info_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_shared_post_id",
                table: "posts",
                column: "shared_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_user_id",
                table: "posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_analytics_id",
                table: "users",
                column: "analytics_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_usersCountingInformation_id",
                table: "users",
                column: "usersCountingInformation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "commentLikes");

            migrationBuilder.DropTable(
                name: "commentReplies");

            migrationBuilder.DropTable(
                name: "commentRepliesLikes");

            migrationBuilder.DropTable(
                name: "commentsCountingInformation");

            migrationBuilder.DropTable(
                name: "deleteLastMessageUsers");

            migrationBuilder.DropTable(
                name: "followers");

            migrationBuilder.DropTable(
                name: "friendRequests");

            migrationBuilder.DropTable(
                name: "friends");

            migrationBuilder.DropTable(
                name: "groupMembers");

            migrationBuilder.DropTable(
                name: "messageConversations");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "postComments");

            migrationBuilder.DropTable(
                name: "postLikes");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "postsCountingInformation");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "analytics");

            migrationBuilder.DropTable(
                name: "usersCounting");
        }
    }
}
