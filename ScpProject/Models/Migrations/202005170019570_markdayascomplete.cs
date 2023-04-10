namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class markdayascomplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompletedAssignedProgramDays", "AthleteId", c => c.Int(nullable: false));
            AddColumn("dbo.CompletedAssignedProgramDays", "WeekNumber", c => c.Int(nullable: false));
            AddColumn("dbo.AthleteProgramHistories", "HideProgramOnHistoryPage", c => c.Boolean(nullable: false));
            AddColumn("dbo.AthleteProgramHistories", "HideSurveyOnProgramHistoryPage", c => c.Boolean(nullable: false));
            CreateIndex("dbo.CompletedAssignedProgramDays", "AthleteId");
            AddForeignKey("dbo.CompletedAssignedProgramDays", "AthleteId", "dbo.Athletes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedAssignedProgramDays", "AthleteId", "dbo.Athletes");
            DropIndex("dbo.CompletedAssignedProgramDays", new[] { "AthleteId" });
            DropColumn("dbo.AthleteProgramHistories", "HideSurveyOnProgramHistoryPage");
            DropColumn("dbo.AthleteProgramHistories", "HideProgramOnHistoryPage");
            DropColumn("dbo.CompletedAssignedProgramDays", "WeekNumber");
            DropColumn("dbo.CompletedAssignedProgramDays", "AthleteId");
        }
    }
}
