using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    
    public class MessageConversationModel
    {

        [Key]
        public int messages_id { get; set; }

        public int? sender_id { get; set; }
        public int? reciever_id { get; set; }
        public int? delete_onlyFor_user_id { get; set; }
        public bool? is_seen { get; set; } = false;
        [MaxLength(500)]
        public string? message_txt { get; set; }

        public int? group_id { get; set; }

        [NotMapped]
        public List<IFormFile>? message_files { get; set; }
        public string? message_files_url { get; set; }

        public DateTime messages_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(reciever_id))]
        public virtual UserModel? user { get; set; }

    }
}
