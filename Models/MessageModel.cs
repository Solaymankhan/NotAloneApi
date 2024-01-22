using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class MessageModel
    {

        [Key]
        public int message_id { get; set; }

        [Required]
        public int? user1_id { get; set; }
        [Required]
        public int? user2_id { get; set; }

        public int? sender_id { get; set; }

        public bool? isText { get; set; }
        [MaxLength(500)]
        public string? last_txt { get; set; }

        public bool? is_read { get; set; } = false;
        public bool? is_group_message { get; set; } = false;

        public bool? new_message_arrive { get; set; } = true;
        public int? group_id { get; set; }

        public DateTime last_messages_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(user1_id))]
        public virtual UserModel? user1 { get; set; }

        [ForeignKey(nameof(user2_id))]
        public virtual UserModel? user2 { get; set; }

        [ForeignKey(nameof(group_id))]
        public virtual GroupsModel? groups { get; set; }

        public virtual List<DeleteLastMessageUserModel>? deleted_last_message_users { get; set; }

    }
}
