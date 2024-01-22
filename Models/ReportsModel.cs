using System.ComponentModel.DataAnnotations;

namespace NotAlone.Models
{
    public class ReportsModel
    {
        [Key]
        public int report_id { set; get; }
        public int? reportedTo_user_id { set; get; }
        public int? reportedBy_user_id { set; get; }

        public int? reported_item_id { set; get; }

        [MaxLength(50)]
        public string? report_type { set; get; }
        [MaxLength(250)]
        public string? report_description_txt { set; get; }

        public DateTime report_time { get; set; } = DateTime.Now;
    }
}
