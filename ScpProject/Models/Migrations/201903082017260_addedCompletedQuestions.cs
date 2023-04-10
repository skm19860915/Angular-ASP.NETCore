namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedCompletedQuestions : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CompletedQuestions", newName: "CompletedQuestionOpenEndeds");
            CreateTable(
                "dbo.CompletedQuestionScales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScaleValue = c.Int(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        ProgramDayItemSurveyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AthleteId)
                .Index(t => t.QuestionId)
                .Index(t => t.AssignedProgramId)
                .Index(t => t.ProgramDayItemSurveyId);
            
            CreateTable(
                "dbo.CompletedQuestionYesNoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YesNoValue = c.Boolean(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AssignedProgramId = c.Int(nullable: false),
                        ProgramDayItemSurveyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AthleteId)
                .Index(t => t.QuestionId)
                .Index(t => t.AssignedProgramId)
                .Index(t => t.ProgramDayItemSurveyId);
            
            AddColumn("dbo.CompletedQuestionOpenEndeds", "Resposne", c => c.String());
            DropColumn("dbo.CompletedQuestionOpenEndeds", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompletedQuestionOpenEndeds", "Value", c => c.String());
            DropIndex("dbo.CompletedQuestionYesNoes", new[] { "ProgramDayItemSurveyId" });
            DropIndex("dbo.CompletedQuestionYesNoes", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedQuestionYesNoes", new[] { "QuestionId" });
            DropIndex("dbo.CompletedQuestionYesNoes", new[] { "AthleteId" });
            DropIndex("dbo.CompletedQuestionScales", new[] { "ProgramDayItemSurveyId" });
            DropIndex("dbo.CompletedQuestionScales", new[] { "AssignedProgramId" });
            DropIndex("dbo.CompletedQuestionScales", new[] { "QuestionId" });
            DropIndex("dbo.CompletedQuestionScales", new[] { "AthleteId" });
            DropColumn("dbo.CompletedQuestionOpenEndeds", "Resposne");
            DropTable("dbo.CompletedQuestionYesNoes");
            DropTable("dbo.CompletedQuestionScales");
            RenameTable(name: "dbo.CompletedQuestionOpenEndeds", newName: "CompletedQuestions");
        }
    }
}
