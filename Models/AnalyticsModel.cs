using System.ComponentModel.DataAnnotations;

namespace NotAlone.Models
{
    public class AnalyticsModel
    {
        [Key]
        public int analytics_id { set; get; }
        [Required]
        public int? user_id { get; set; }

        public string? hash_tags { get; set; }
        public string? analytics_user_ids { get; set; }
        public string? analytics_group_ids { get; set; }
        public int? total_new_notifications { get; set; } = 0;
        public int? total_new_messages { get; set; } = 0;
        public int? total_new_reports { get; set; } = 0;
        public int? total_new_friend_requests { get; set; } = 0;
        public bool? is_active { get; set; } = true;
        public DateTime last_active_time { get; set; } = DateTime.Now;
    }
}
