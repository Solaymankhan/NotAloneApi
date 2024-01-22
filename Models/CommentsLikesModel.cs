using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class CommentsLikesModel
    {
        [Key]
        public int liked_id { get; set; }

        public int? user_id { get; set; }
        public int? comment_id { get; set; }

        public DateTime liked_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(user_id))]
        public virtual UserModel? user { get; set; }
    }
}
