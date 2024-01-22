using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class DeleteLastMessageUserModel
    {
        [Key]
        public int delete_last_message_userId { get; set; }

        public int message_id { get; set; }

        public int user_id { get; set; }
        public DateTime delete_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(message_id))]
        public virtual MessageModel? last_message { get; set; }



    }
}
