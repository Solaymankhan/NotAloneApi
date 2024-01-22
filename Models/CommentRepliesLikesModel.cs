using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NotAlone.Models
{
    public class CommentRepliesLikesModel
    {

        [Key]
        public int comment_replies_liked_id { get; set; }

        public int? user_id { get; set; }
        public int? comment_reply_id { get; set; }

        public DateTime liked_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(user_id))]
        public virtual UserModel? user { get; set; }
    }
}
