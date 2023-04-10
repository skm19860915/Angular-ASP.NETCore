namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class moreDataForCompletedMetric : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CompletedQuestions", name: "OriginalQuestionId", newName: "QuestionId");
            RenameIndex(table: "dbo.CompletedQuestions", name: "IX_OriginalQuestionId", newName: "IX_QuestionId");
            AddColumn("dbo.CompletedMetrics", "WeekId", c => c.Int(nullable: false));
            AddColumn("dbo.CompletedQuestions", "ProgramDayItemSurveyId", c => c.Int(nullable: false));
            CreateIndex("dbo.CompletedQuestions", "ProgramDayItemSurveyId");
            AddForeignKey("dbo.CompletedQuestions", "ProgramDayItemSurveyId", "dbo.ProgramDayItemSurveys", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedQuestions", "ProgramDayItemSurveyId", "dbo.ProgramDayItemSurveys");
            DropIndex("dbo.CompletedQuestions", new[] { "ProgramDayItemSurveyId" });
            DropColumn("dbo.CompletedQuestions", "ProgramDayItemSurveyId");
            DropColumn("dbo.CompletedMetrics", "WeekId");
            RenameIndex(table: "dbo.CompletedQuestions", name: "IX_QuestionId", newName: "IX_OriginalQuestionId");
            RenameColumn(table: "dbo.CompletedQuestions", name: "QuestionId", newName: "OriginalQuestionId");
        }
    }
}
