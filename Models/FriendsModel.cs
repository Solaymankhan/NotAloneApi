using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class FriendsModel
    {
        [Key]
        public int friendship_id { get; set; }
        public int? user1_id { get; set; }
        public int? user2_id { get; set; }

        [ForeignKey(nameof(user1_id))]
        public virtual UserModel? user1 { get; set; }
        [ForeignKey(nameof(user2_id))]
        public virtual UserModel? user2 { get; set; }
        public DateTime friendship_time { get; set; } = DateTime.Now;
    }
}
