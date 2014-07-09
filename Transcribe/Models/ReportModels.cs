using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class ReportModels
    {
        [Key]
        public int ReportId { get; set; }
        [Display(Name = "Record")]
        public int RecordId { get; set; }
        [Display(Name = "Report")]
        public string report { get; set; }
        [Display(Name = "Submitted")]
        public DateTime commit { get; set; }
    }
}