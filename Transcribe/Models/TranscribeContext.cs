using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Transcribe.Models
{
    public class TranscribeContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public TranscribeContext() : base("name=TranscribeContext")
        {
        }

        public DbSet<Transcribe.Models.RecordModels> RecordModels { get; set; }
        public DbSet<Transcribe.Models.VersionModels> VersionModels { get; set; }
        public DbSet<Transcribe.Models.CommentsModels> CommentsModels { get; set; }
        public DbSet<Transcribe.Models.FavoritesModels> FavoritesModels { get; set; }
        public DbSet<Transcribe.Models.ActivityModels> ActivityModels { get; set; }
        public DbSet<Transcribe.Models.SessionModels> SessionModels { get; set; }
        public DbSet<Transcribe.Models.ReportModels> ReportModels { get; set; }
        public DbSet<Transcribe.Models.CompletedModels> CompletedModels { get; set; }
    }
}
