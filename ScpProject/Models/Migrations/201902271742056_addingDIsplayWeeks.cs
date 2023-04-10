namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingDIsplayWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MetricDisplayWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MetricId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .Index(t => t.MetricId);
            
            CreateTable(
                "dbo.NoteDisplayWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramDayItemNoteId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItemNotes", t => t.ProgramDayItemNoteId)
                .Index(t => t.ProgramDayItemNoteId);
            
            CreateTable(
                "dbo.SurveyDisplayWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Surveys", t => t.SurveyId)
                .Index(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyDisplayWeeks", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.NoteDisplayWeeks", "ProgramDayItemNoteId", "dbo.ProgramDayItemNotes");
            DropForeignKey("dbo.MetricDisplayWeeks", "MetricId", "dbo.Metrics");
            DropIndex("dbo.SurveyDisplayWeeks", new[] { "SurveyId" });
            DropIndex("dbo.NoteDisplayWeeks", new[] { "ProgramDayItemNoteId" });
            DropIndex("dbo.MetricDisplayWeeks", new[] { "MetricId" });
            DropTable("dbo.SurveyDisplayWeeks");
            DropTable("dbo.NoteDisplayWeeks");
            DropTable("dbo.MetricDisplayWeeks");
        }
    }
}
