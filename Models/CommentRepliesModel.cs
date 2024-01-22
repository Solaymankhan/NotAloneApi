using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class CommentRepliesModel
    {
        [Key]
        public int replied_id { get; set; }

        public int? user_id { get; set; }
        public int? comment_id { get; set; }

        [Required]
        [MaxLength(500)]
        public string replied_txt { get; set; }
        public int? total_likes { get; set; } = 0;

        public DateTime repleid_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(user_id))]
        public virtual UserModel? user { get; set; }
    }
}
