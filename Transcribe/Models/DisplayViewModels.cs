using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Transcribe.Models
{
    public class HomeViewModel
    {
        public int percentage { get; set; } 
        public int totalRecords { get; set; }
        public int progress { get; set; }
        public int updates { get; set; }
        public virtual ICollection<ActivityModels> Activities { get; set; }
        public virtual ICollection<RecordModels> Records { get; set; }
    }

    public class BrowseViewModel
    {
        public BrowseViewModel(string RecordTitle)
        {
          this.RecordTitle = RecordTitle;
        }

        public BrowseViewModel() { }

        public string RecordTitle { get; set; }
        public int total { get; set; }
        public int totalTranscribed { get; set; }
    }

    public class CorrectionsViewModel
    {
        public int RecordId { get; set; }
        public string title { get; set; }
        public string status { get; set; }
        public string statusBtn { get; set; }
        public string statusLayer { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string image { get; set; }
        public string diff { get; set; }
        public string diffBtn { get; set; }
        public string sysid { get; set; }
        public string name { get; set; }
        public string oldValue { get; set; }
        public string value { get; set; }
        public int pk { get; set; }
        public string session { get; set; }
        public int pageno { get; set; }
        public int totalpages { get; set; }
        public int nextpage { get; set; }
        public int prevpage { get; set; }
        public int nextUntranscribed { get; set; }
        public string nextDisabled { get; set; }
        public string prevDisabled { get; set; }
        public string nextUnDisabled { get; set; }
        public string favorite { get; set; }
        public string type { get; set; }
        public string email { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Report")]
        public string reportDetails { get; set; }

        public virtual ICollection<VersionModels> Versions { get; set; }
        
    }

    public class ControlsViewModel
    {
        public int id { get; set; }

        [Required]
        public string comment { get; set; }       
    }

    public class ActivityViewModel
    {
        public int id { get; set; }
        public virtual ICollection<ActivityModels> Activities { get; set; }
    }
    public class CommentViewModel
    {
        public int id { get; set; }
        public virtual ICollection<CommentsModels> Comments { get; set; }
        
    }
    public class ExportViewModel
    {
        public int RecordId { get; set; }
    }

    public class FavoriteViewModel
    {
        public virtual ICollection<FavoritesModels> Favorites { get; set; }
    }
    public class CompletedViewModel
    {
        public CompletedModels Completed { get; set; }
        public RecordModels Records { get; set; }
    }

    public class HeartsViewModel
    {
        public int hearts { get; set; }
    }

    public class LeaderBoardViewModel
    {
        public string userid { get; set; }
        public int total { get; set; }
    }

    public class UserViewModel
    {
        public string userid { get; set; }
        public string uniqueUserId { get; set; }
    }
}