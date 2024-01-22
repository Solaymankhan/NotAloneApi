using System.ComponentModel.DataAnnotations;

namespace NotAlone.Models
{
    public class UsersCountingInformationModel
    {

        [Key]
        public int usersCountingInformation_id { get; set; }
        public int? user_id { get; set; }

        public int? total_friends { get; set; } = 0;

        public int? total_followers { get; set; } = 0;
        public int? total_followings { get; set; } = 0;
    }
}
