using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class GroupsModel
    {

        [Key]
        public int group_id { set; get; }
        public int? group_admin_id { set; get; }
        public bool? is_group_public { set; get; } = true;
        [MaxLength(250)]
        public string? gorup_description_txt { set; get; }
        [NotMapped]
        public IFormFile? group_img_file { get; set; }
        public string? group_img_url { get; set; }
        [NotMapped]
        public IFormFile? group_bg_img_file { get; set; }
        public string? group_bg_img_url { get; set; }
        [MaxLength(30)]
        public string? gorup_name { set; get; }

        public int? total_members { set; get; } = 0;

        public DateTime group_creation_time { get; set; } = DateTime.Now;

        [ForeignKey(nameof(group_admin_id))]
        public virtual UserModel? groupAdmin { get; set; }

        public virtual List<GroupMembersModel>? members { get; set; }


    }
}
