using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class RecordModels
    {
        [Key]
        public int RecordId { get; set; }
        [Display(Name = "Title")]
        public string title { get; set; }
        [Display(Name = "Start date")]
        public string startdate { get; set; }
        [Display(Name = "End date")]
        public string enddate { get; set; }
        [Display(Name = "Difficulty")]
        public string difficulty { get; set; }
        [Display(Name = "Page number")]
        public int page { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Other system identifier")]
        public string sysident { get; set; }
        [Display(Name = "Text type")]
        public string type { get; set; }
        [Display(Name = "Image name")]
        public string image { get; set; }
        [Display(Name = "Commit date")]
        public DateTime commit { get; set; }

        public virtual ICollection<CommentsModels> Comments { get; set; }
        public virtual ICollection<ActivityModels> Activities { get; set; }
        public virtual ICollection<VersionModels> Versions { get; set; }
        public virtual ICollection<FavoritesModels> Favorites { get; set; }
        public virtual ICollection<SessionModels> Sessions { get; set; }
        public virtual ICollection<ReportModels> Reports { get; set; }
        public virtual ICollection<CompletedModels> Complete { get; set; }
        
    }
}