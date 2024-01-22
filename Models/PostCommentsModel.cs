using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class PostCommentsModel
    {
        [Key]
        public int comment_id { get; set; }

        public int? user_id { get; set; }
        public int? post_id { get; set; }

        [Required]
        [MaxLength(500)]
        public string? comment_txt { get; set; }

        [ForeignKey(nameof(user_id))]
        public virtual UserModel? user { get; set; }

        public DateTime comment_time { get; set; } = DateTime.Now;
    }
}
