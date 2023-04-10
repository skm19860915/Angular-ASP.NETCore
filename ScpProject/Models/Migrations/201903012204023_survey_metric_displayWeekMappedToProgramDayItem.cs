namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class survey_metric_displayWeekMappedToProgramDayItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MetricDisplayWeeks", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.SurveyDisplayWeeks", "SurveyId", "dbo.Surveys");
            DropIndex("dbo.MetricDisplayWeeks", new[] { "MetricId" });
            DropIndex("dbo.SurveyDisplayWeeks", new[] { "SurveyId" });
            AddColumn("dbo.MetricDisplayWeeks", "ProgramDayItemMeticId", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyDisplayWeeks", "ProgramDayItemSurveyId", c => c.Int(nullable: false));
            CreateIndex("dbo.MetricDisplayWeeks", "ProgramDayItemMeticId");
            CreateIndex("dbo.SurveyDisplayWeeks", "ProgramDayItemSurveyId");
            AddForeignKey("dbo.MetricDisplayWeeks", "ProgramDayItemMeticId", "dbo.ProgramDayItemMetrics", "Id");
            AddForeignKey("dbo.SurveyDisplayWeeks", "ProgramDayItemSurveyId", "dbo.ProgramDayItemSurveys", "Id");
            DropColumn("dbo.MetricDisplayWeeks", "MetricId");
            DropColumn("dbo.SurveyDisplayWeeks", "SurveyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyDisplayWeeks", "SurveyId", c => c.Int(nullable: false));
            AddColumn("dbo.MetricDisplayWeeks", "MetricId", c => c.Int(nullable: false));
            DropForeignKey("dbo.SurveyDisplayWeeks", "ProgramDayItemSurveyId", "dbo.ProgramDayItemSurveys");
            DropForeignKey("dbo.MetricDisplayWeeks", "ProgramDayItemMeticId", "dbo.ProgramDayItemMetrics");
            DropIndex("dbo.SurveyDisplayWeeks", new[] { "ProgramDayItemSurveyId" });
            DropIndex("dbo.MetricDisplayWeeks", new[] { "ProgramDayItemMeticId" });
            DropColumn("dbo.SurveyDisplayWeeks", "ProgramDayItemSurveyId");
            DropColumn("dbo.MetricDisplayWeeks", "ProgramDayItemMeticId");
            CreateIndex("dbo.SurveyDisplayWeeks", "SurveyId");
            CreateIndex("dbo.MetricDisplayWeeks", "MetricId");
            AddForeignKey("dbo.SurveyDisplayWeeks", "SurveyId", "dbo.Surveys", "Id");
            AddForeignKey("dbo.MetricDisplayWeeks", "MetricId", "dbo.Metrics", "Id");
        }
    }
}
