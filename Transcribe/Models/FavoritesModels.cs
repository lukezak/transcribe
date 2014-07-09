using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class FavoritesModels
    {
        [Key]
        public int FavoriteId { get; set; }
        public int RecordId { get; set; }
        public string title { get; set; }
        public string userId { get; set; }
    }
}