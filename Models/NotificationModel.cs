using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class NotificationModel
    {

        [Key]
        public int notification_id { get; set; }
        public int? to_user_id { get; set; }
        public int? from_user_id { get; set; }

        // Relevent notification table id
        [Required]
        public int? source_id { get; set; }
        [MaxLength(50)]
        public string? notification_type { get; set; }

        public bool? is_read { get; set; } = false;

        [ForeignKey(nameof(from_user_id))]
        public virtual UserModel? fromUser { get; set; }

        public DateTime notification_time { get; set; } = DateTime.Now;
    }
}
