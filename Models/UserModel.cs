using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace NotAlone.Models
{
    public class UserModel
    {
 
        [Key]
        public int user_id {  get; set; }
        [MaxLength(30)]
        public string? first_name { get; set; }
        [MaxLength(30)]
        public string? last_name { get; set;}

        [EmailAddress]
        [MaxLength(50)]
        public string? email { get; set; }
        [MaxLength(20)]
        public string? password { get; set; }
        public string? profile_picture_url { get; set; }
        [NotMapped]
        public IFormFile? profile_picture_file { get; set; }
        public string? baground_picture_url { get; set; }
        [NotMapped]
        public IFormFile? baground_picture_file { get; set; }
        [MaxLength(100)]
        public string? address { get; set; }
        public DateTime user_registration_time { get; set; } = DateTime.Now;
        public int? analytics_id { get; set; }
        public int? usersCountingInformation_id { get; set; }

        [ForeignKey(nameof(usersCountingInformation_id))]
        public virtual UsersCountingInformationModel? usersCounting { get; set; }
        [ForeignKey(nameof(analytics_id))]
        public virtual AnalyticsModel? analytics { get; set; }

    }
}
