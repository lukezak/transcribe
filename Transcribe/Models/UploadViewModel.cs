using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Transcribe.Models
{
    public class UploadViewModel
    {
        [Required]
        [Display(Name = "File input")]
        public HttpPostedFileBase file { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Required]
        [Display(Name = "Start date range")]
        public string startdate { get; set; }

        [Required]
        [Display(Name = "End date range")]
        public string enddate { get; set; }

        [Display(Name = "Other system identifier (URL)")]
        [DataType(DataType.Url)]
        public string sysident { get; set; }

        [Required]
        [Display(Name = "Difficulty")]
        public string difficulty { get; set; }

        [Required]
        [Display(Name = "Page number")]
        public int pageno { get; set; }

        [Required]
        [Display(Name = "Text type")]
        public string type { get; set; }

        [Display(Name = "Keep current data")]
        public string persist { get; set; }

        public string hdncoords { get; set; }
    }
}