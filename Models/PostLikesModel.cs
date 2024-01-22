using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class PostLikesModel
    {
        [Key]
        public int likes_id { get; set; }

        public int? user_id { get; set; }
        public int? post_id { get; set; }
        [ForeignKey(nameof(user_id))]
        public virtual UserModel? user { get; set; }
        public DateTime liked_time { get; set; } = DateTime.Now;
    }
}
