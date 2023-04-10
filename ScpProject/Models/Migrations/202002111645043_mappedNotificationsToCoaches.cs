namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class mappedNotificationsToCoaches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScaleThresholdToCoaches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ScaleThresholdId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.ScaleQuestionThresholds", t => t.ScaleThresholdId)
                .Index(t => t.UserId)
                .Index(t => t.ScaleThresholdId);
            
            CreateTable(
                "dbo.YesNoQuestionThresholdToCoaches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        YesNoThresholdId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.YesNoQuestionThresholds", t => t.YesNoThresholdId)
                .Index(t => t.UserId)
                .Index(t => t.YesNoThresholdId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YesNoQuestionThresholdToCoaches", "YesNoThresholdId", "dbo.YesNoQuestionThresholds");
            DropForeignKey("dbo.YesNoQuestionThresholdToCoaches", "UserId", "dbo.Users");
            DropForeignKey("dbo.ScaleThresholdToCoaches", "ScaleThresholdId", "dbo.ScaleQuestionThresholds");
            DropForeignKey("dbo.ScaleThresholdToCoaches", "UserId", "dbo.Users");
            DropIndex("dbo.YesNoQuestionThresholdToCoaches", new[] { "YesNoThresholdId" });
            DropIndex("dbo.YesNoQuestionThresholdToCoaches", new[] { "UserId" });
            DropIndex("dbo.ScaleThresholdToCoaches", new[] { "ScaleThresholdId" });
            DropIndex("dbo.ScaleThresholdToCoaches", new[] { "UserId" });
            DropTable("dbo.YesNoQuestionThresholdToCoaches");
            DropTable("dbo.ScaleThresholdToCoaches");
        }
    }
}
