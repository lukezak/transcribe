using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class SessionModels
    {
        [Key]
        public long SessionId { get; set; }
        public int RecordId { get; set; }
        public string currentSession { get; set; }
        public DateTime commit { get; set; }
    }
}