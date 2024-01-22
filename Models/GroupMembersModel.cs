using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class GroupMembersModel
    {
        [Key]
        public int group_membership_id { set; get; }

        public int? group_id { set; get; }
        public int? user_id { set; get; }


        //Requested,Invited,Joined
        [MaxLength(20)]
        public string? membership_status { set; get; } = "Invited";
        public DateTime membership_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(group_id))]
        public GroupsModel? group { set; get; }

        [ForeignKey(nameof(user_id))]
        public UserModel? user { set; get; }
    }
}
