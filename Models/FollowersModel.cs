using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace NotAlone.Models
{
    public class FollowersModel
    {
 
        [Key]
        public int followers_id {  get; set; }
        public int? follower_user_id { get; set; }
        public int? following_user_id { get; set; }

        [ForeignKey(nameof(follower_user_id))]
        public virtual UserModel? follower_user { get; set; }

        [ForeignKey(nameof(following_user_id))]
        public virtual UserModel? following_user { get; set; }
        public DateTime following_time { get; set; } = DateTime.Now;

    }
}
