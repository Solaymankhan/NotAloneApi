using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotAlone.Models
{
    public class PostModel
    {
        [Key]
        public int post_id { get; set; }
        public int? user_id { get; set; }

        [MaxLength(500)]
        public string? text { get; set; }

        public string? post_file_urls { get; set; }

        [MaxLength(100)]
        public string? place { get; set; }

        [NotMapped]
        public List<IFormFile>? post_files { get; set; }
        public bool? is_group_post { get; set; } = false;

        public bool? is_shared_post { get; set; } = false;

        [NotMapped]
        public bool? is_liked { get; set; } = false;

        public int? shared_post_id { get; set; }

        public int? group_id { get; set; }
        public int? posts_counting_info_id { get; set; }

        public DateTime post_time { get; set; } = DateTime.Now;

       
        [ForeignKey(nameof(posts_counting_info_id))]
        public virtual PostsCountingInformationModel? postsCountingInformation { get; set; }

        [ForeignKey(nameof(user_id))]
        public virtual UserModel? users { get; set; }

        [ForeignKey(nameof(group_id))]
        public virtual GroupsModel? groups { get; set; }

        [ForeignKey(nameof(shared_post_id))]
        public virtual PostModel? sharedPost { get; set; }

    }
}
