using System.ComponentModel.DataAnnotations;

namespace NotAlone.Models
{
    public class CommentsCountingInformationModel
    {

        [Key]
        public int comments_counting_info_id { get; set; }
        public int? comments_id { get; set; }
        public int? total_likes { get; set; } = 0;

        public int? total_replies { get; set; } = 0;
    }
}
