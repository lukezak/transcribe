namespace Transcribe.Migrations.TranscribeContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityModels",
                c => new
                    {
                        ActivityId = c.Long(nullable: false, identity: true),
                        VersionId = c.Long(nullable: false),
                        RecordId = c.Int(nullable: false),
                        userId = c.String(),
                        uniqueUserId = c.String(),
                        oldline = c.String(),
                        newline = c.String(),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.CommentsModels",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        userId = c.String(),
                        comment = c.String(),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.CompletedModels",
                c => new
                    {
                        CompletedId = c.Long(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        userId = c.String(),
                        uniqueUserId = c.String(),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CompletedId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.FavoritesModels",
                c => new
                    {
                        FavoriteId = c.Int(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        title = c.String(),
                        userId = c.String(),
                    })
                .PrimaryKey(t => t.FavoriteId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.RecordModels",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        startdate = c.String(),
                        enddate = c.String(),
                        difficulty = c.String(),
                        page = c.Int(nullable: false),
                        status = c.String(),
                        sysident = c.String(),
                        type = c.String(),
                        image = c.String(),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId);
            
            CreateTable(
                "dbo.ReportModels",
                c => new
                    {
                        ReportId = c.Int(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        report = c.String(),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReportId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.SessionModels",
                c => new
                    {
                        SessionId = c.Long(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        currentSession = c.String(),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SessionId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.VersionModels",
                c => new
                    {
                        VersionId = c.Long(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        userId = c.String(),
                        transcription = c.String(),
                        sortorder = c.Int(nullable: false),
                        commit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VersionId)
                .ForeignKey("dbo.RecordModels", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VersionModels", "RecordId", "dbo.RecordModels");
            DropForeignKey("dbo.SessionModels", "RecordId", "dbo.RecordModels");
            DropForeignKey("dbo.ReportModels", "RecordId", "dbo.RecordModels");
            DropForeignKey("dbo.FavoritesModels", "RecordId", "dbo.RecordModels");
            DropForeignKey("dbo.CompletedModels", "RecordId", "dbo.RecordModels");
            DropForeignKey("dbo.CommentsModels", "RecordId", "dbo.RecordModels");
            DropForeignKey("dbo.ActivityModels", "RecordId", "dbo.RecordModels");
            DropIndex("dbo.VersionModels", new[] { "RecordId" });
            DropIndex("dbo.SessionModels", new[] { "RecordId" });
            DropIndex("dbo.ReportModels", new[] { "RecordId" });
            DropIndex("dbo.FavoritesModels", new[] { "RecordId" });
            DropIndex("dbo.CompletedModels", new[] { "RecordId" });
            DropIndex("dbo.CommentsModels", new[] { "RecordId" });
            DropIndex("dbo.ActivityModels", new[] { "RecordId" });
            DropTable("dbo.VersionModels");
            DropTable("dbo.SessionModels");
            DropTable("dbo.ReportModels");
            DropTable("dbo.RecordModels");
            DropTable("dbo.FavoritesModels");
            DropTable("dbo.CompletedModels");
            DropTable("dbo.CommentsModels");
            DropTable("dbo.ActivityModels");
        }
    }
}
