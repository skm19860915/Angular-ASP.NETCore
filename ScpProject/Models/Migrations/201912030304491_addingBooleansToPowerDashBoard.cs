namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingBooleansToPowerDashBoard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "VisitedExercise", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VisitedPrograms", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VistedRosters", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VisitedSurveys", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VisitedSetsReps", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VisitedCoachRoster", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VisitedMetrics", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "VisitedProgramBuilder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "VisitedProgramBuilder");
            DropColumn("dbo.Users", "VisitedMetrics");
            DropColumn("dbo.Users", "VisitedCoachRoster");
            DropColumn("dbo.Users", "VisitedSetsReps");
            DropColumn("dbo.Users", "VisitedSurveys");
            DropColumn("dbo.Users", "VistedRosters");
            DropColumn("dbo.Users", "VisitedPrograms");
            DropColumn("dbo.Users", "VisitedExercise");
        }
    }
}
