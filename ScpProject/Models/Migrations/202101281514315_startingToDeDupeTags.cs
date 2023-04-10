namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class startingToDeDupeTags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AthleteTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.ExerciseTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.MetricTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.MovieTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.MultiMediaTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.ProgramTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyTags", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.WorkoutTags", "OrganizationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkoutTags", "OrganizationId");
            DropColumn("dbo.SurveyTags", "OrganizationId");
            DropColumn("dbo.ProgramTags", "OrganizationId");
            DropColumn("dbo.MultiMediaTags", "OrganizationId");
            DropColumn("dbo.MovieTags", "OrganizationId");
            DropColumn("dbo.MetricTags", "OrganizationId");
            DropColumn("dbo.ExerciseTags", "OrganizationId");
            DropColumn("dbo.AthleteTags", "OrganizationId");
        }
    }
}
