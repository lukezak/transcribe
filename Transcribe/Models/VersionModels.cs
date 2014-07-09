using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class VersionModels
    {
        [Key]
        public long VersionId { get; set; }
        public int RecordId { get; set; }
        public string userId { get; set; }
        public string transcription { get; set; }
        public int sortorder { get; set; }
        public DateTime commit { get; set; }
        
    }
}