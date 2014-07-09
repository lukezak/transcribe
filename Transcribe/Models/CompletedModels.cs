using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class CompletedModels
    {
        [Key]
        public long CompletedId { get; set; }
        public int RecordId { get; set; }
        public string userId { get; set; }
        public string uniqueUserId { get; set; }
        public DateTime commit { get; set; }
    }
}