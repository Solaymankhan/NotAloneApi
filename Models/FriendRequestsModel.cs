using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class FriendRequestsModel
    {
        [Key]
   
        public int request_id { get; set; }
        public int? sender_id { get; set; }
        public int? reciever_id { get; set; }
        public bool? request_status { get; set; } = false;

     
        [ForeignKey(nameof(sender_id))]
        public virtual UserModel? sender { get; set; }

        [ForeignKey(nameof(reciever_id))]
        public virtual UserModel? reciever { get; set; }

        public DateTime request_time { get; set; } = DateTime.Now;
    }
}
