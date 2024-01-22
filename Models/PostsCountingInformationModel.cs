using System.ComponentModel.DataAnnotations;

namespace NotAlone.Models
{
    public class PostsCountingInformationModel
    {
        [Key]
        public int posts_counting_info_id { get; set; }
        public int? post_id { get; set; }
        public int? total_likes { get; set; } = 0;
        public int? total_comments { get; set; } = 0;
        public int? total_shares { get; set; } = 0;

        internal void Update()
        {
            throw new NotImplementedException();
        }
    }
}
