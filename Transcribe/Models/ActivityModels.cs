using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class ActivityModels
    {
        [Key]
        public long ActivityId { get; set; }
        public long VersionId { get; set; }
        public int RecordId { get; set; }
        public string userId { get; set; }
        public string uniqueUserId { get; set; }
        public string oldline { get; set; }
        public string newline { get; set; }
        public DateTime commit { get; set; }
    }
}